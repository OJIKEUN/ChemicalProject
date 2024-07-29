using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using ChemicalProject.Data;
using ChemicalProject.Models;

namespace ChemicalProject.Controllers
{
    public class StockCheckController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StockCheckController> _logger;

        public StockCheckController(ApplicationDbContext context, ILogger<StockCheckController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CheckStock()
        {
            try
            {
                await CheckChemicalStock();
                _logger.LogInformation("Stock check completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during stock check");
            }
        }

        private async Task CheckChemicalStock()
        {
            var chemicals = await _context.Chemicals
                .Include(c => c.Area)
                .ToListAsync();

            var chemicalsToNotify = new List<(Chemical_FALab Chemical, int CurrentStock)>();

            foreach (var chemical in chemicals)
            {
                var records = await _context.Records
                    .Where(r => r.ChemicalId == chemical.Id)
                    .ToListAsync();

                var currentStock = records.Sum(r => r.ReceivedQuantity) - records.Sum(r => r.Consumption);

                if (currentStock <= chemical.MinimumStock)
                {
                    chemicalsToNotify.Add((chemical, currentStock));
                }
            }

            _logger.LogInformation($"Found {chemicalsToNotify.Count} chemicals with low stock.");

            foreach (var (chemical, currentStock) in chemicalsToNotify)
            {
                _logger.LogInformation($"Chemical {chemical.ChemicalName} has current stock {currentStock}, minimum stock {chemical.MinimumStock}");
                await SendLowStockEmail(chemical, currentStock);
            }
        }

        private async Task SendLowStockEmail(Chemical_FALab chemical, int currentStock)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ESH Notification", "bth-esh@infineon.com"));

            var userArea = await _context.UserAreas
                .FirstOrDefaultAsync(u => u.AreaId == chemical.AreaId);

            if (userArea == null)
            {
                _logger.LogWarning($"No UserArea found for AreaId: {chemical.AreaId}");
                return;
            }

            if (!string.IsNullOrWhiteSpace(userArea.EmailManager))
            {
                message.To.Add(MailboxAddress.Parse(userArea.EmailManager));
            }
            else
            {
                _logger.LogWarning($"No manager email found for AreaId: {chemical.AreaId}");
            }

            if (!string.IsNullOrWhiteSpace(userArea.EmailUser))
            {
                message.To.Add(MailboxAddress.Parse(userArea.EmailUser));
            }
            else
            {
                _logger.LogWarning($"No user email found for AreaId: {chemical.AreaId}");
            }

            message.Cc.Add(MailboxAddress.Parse("Andas.Puranda@infineon.com"));
            message.Cc.Add(MailboxAddress.Parse("Agung.Sanjaya@infineon.com"));

            message.Subject = $"Low Stock Alert: {chemical.ChemicalName}";
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
            <p>Dear All,</p>
            <p>This is to notify you that the stock of chemical {chemical.ChemicalName} in {chemical.Area.Name} has reached or fallen below the minimum stock level.</p>
            <p>Current Stock: {currentStock}</p>
            <p>Minimum Stock: {chemical.MinimumStock}</p>
            <p>Please take necessary actions to replenish the stock.</p>
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

                _logger.LogInformation($"Low stock email sent for chemical {chemical.ChemicalName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send low stock email for chemical {chemical.ChemicalName}");
            }
        }
    }
}