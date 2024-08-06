using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ChemicalProject.Data;
using MimeKit;
using MailKit.Net.Smtp;
using ChemicalProject.Models;
using Microsoft.Extensions.Logging;

namespace ChemicalProject.Controllers
{
    public class ExpireCheckController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExpireCheckController> _logger;

        public ExpireCheckController(ApplicationDbContext context, ILogger<ExpireCheckController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CheckExpiration()
        {
            try
            {
                await CheckChemicals();
                _logger.LogInformation("Expiration check completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during expiration check");
            }
        }

        private async Task CheckChemicals()
        {
            var today = DateTime.Today;
            var exactlyTwoMonthsFromNow = today.AddDays(60);

            var recordsToNotify = await _context.Records
                .Where(r => r.ExpiredDate.HasValue &&
                            (r.ExpiredDate.Value.Date == exactlyTwoMonthsFromNow ||
                             r.ExpiredDate.Value.Date <= today))
                .Include(r => r.Chemical_FALab)
                .ThenInclude(c => c.Area)
                .ToListAsync();

            _logger.LogInformation($"Found {recordsToNotify.Count} records to check for expiration.");

            foreach (var record in recordsToNotify)
            {
                if (record.ExpiredDate.Value.Date == exactlyTwoMonthsFromNow)
                {
                    _logger.LogInformation($"Sending TwoMonthWarning for chemical {record.Chemical_FALab.ChemicalName} expiring on {record.ExpiredDate.Value:yyyy-MM-dd}");
                    await SendExpirationEmail(record, "TwoMonthWarning");
                }
                else if (record.ExpiredDate.Value.Date <= today)
                {
                    _logger.LogInformation($"Sending Expired notification for chemical {record.Chemical_FALab.ChemicalName} expired on {record.ExpiredDate.Value:yyyy-MM-dd}");
                    await SendExpirationEmail(record, "Expired");
                }
            }
        }

        private async Task SendExpirationEmail(Records_FALab record, string notificationType)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ESH Notification", "bth-esh@infineon.com"));

            var userArea = await _context.UserAreas
                .FirstOrDefaultAsync(u => u.AreaId == record.Chemical_FALab.AreaId);

            if (userArea == null)
            {
                _logger.LogWarning($"No UserArea found for AreaId: {record.Chemical_FALab.AreaId}");
                return;
            }

            if (!string.IsNullOrWhiteSpace(userArea.EmailManager))
            {
                message.To.Add(MailboxAddress.Parse(userArea.EmailManager));
            }
            else
            {
                _logger.LogWarning($"No manager email found for AreaId: {record.Chemical_FALab.AreaId}");
            }

            if (!string.IsNullOrWhiteSpace(userArea.EmailUser))
            {
                message.To.Add(MailboxAddress.Parse(userArea.EmailUser));
            }
            else
            {
                _logger.LogWarning($"No user email found for AreaId: {record.Chemical_FALab.AreaId}");
            }

            message.Cc.Add(MailboxAddress.Parse("Andas.Puranda@infineon.com"));
            message.Cc.Add(MailboxAddress.Parse("Agung.Sanjaya@infineon.com"));
            message.Cc.Add(MailboxAddress.Parse("NOVA.ASTUTI@infineon.com"));

            var expiryStatus = notificationType == "TwoMonthWarning" ? "Will expire in 2 months" : "Has expired";

            message.Subject = $"Chemical Expired Alert: {record.Chemical_FALab.ChemicalName}";
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
            <h4>Dear All,</h4>
            <p>This alert notifies you that,</p>
            <p>Chemical Name: {record.Chemical_FALab.ChemicalName}. <br>Area Name: {record.Chemical_FALab.Area.Name} . <br> Expiry Date: {record.ExpiredDate.Value:yyyy-MM-dd}. </p>
            <p>{expiryStatus}. Prompt action is required to ensure a safe and compliant response. <br>Please take necessary steps to ensure the safe handling and disposal of this expired chemical, <br> and update the inventory accordingly. </p>
            <h4>Regards,<br>Chemical Monitoring System</h4>";

            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("mailrelay-internal.infineon.com", 25, false);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Expiration email sent for chemical {record.Chemical_FALab.ChemicalName}, type: {notificationType}, to: {string.Join(", ", message.To)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send expiration email for chemical {record.Chemical_FALab.ChemicalName}, type: {notificationType}");
            }
        }
    }
}