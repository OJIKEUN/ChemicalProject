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
            var twoMonthsBeforeExpiry = today.AddDays(60);

            var recordsToNotify = await _context.Records
                .Where(r => r.ExpiredDate.HasValue &&
                            (r.ExpiredDate.Value.Date == today ||
                             r.ExpiredDate.Value.Date == twoMonthsBeforeExpiry ||
                             r.ExpiredDate.Value.Date < today))
                .Include(r => r.Chemical_FALab)
                .ThenInclude(c => c.Area)
                .ToListAsync();

            foreach (var record in recordsToNotify)
            {
                if (record.ExpiredDate.Value.Date == twoMonthsBeforeExpiry)
                {
                    await SendExpirationEmail(record, "TwoMonthWarning");
                }
                else if (record.ExpiredDate.Value.Date <= today)
                {
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

            var expiryStatus = notificationType == "TwoMonthWarning" ? "will expire in 2 months" : "has expired";

            message.Subject = $"Chemical Expiration Notice: {record.Chemical_FALab.ChemicalName}";
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
    <p>Dear All,</p>
    <p>This is to notify you that the chemical {record.Chemical_FALab.ChemicalName} in {record.Chemical_FALab.Area.Name} {expiryStatus}.</p>
    <p>Expiry Date: {record.ExpiredDate.Value:yyyy-MM-dd}</p>
    <p>Please take necessary actions.</p>
    <p>Regards,<br>ESH Notification System</p>";

            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("mailrelay-internal.infineon.com", 25, false);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Expiration email sent for chemical {record.Chemical_FALab.ChemicalName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send expiration email for chemical {record.Chemical_FALab.ChemicalName}");
            }
        }
    }
}