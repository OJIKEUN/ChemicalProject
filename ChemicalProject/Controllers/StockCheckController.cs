﻿using Microsoft.EntityFrameworkCore;
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

                // Periksa apakah ada record untuk chemical ini
                if (records.Any())
                {
                    var currentStock = records.Sum(r => r.ReceivedQuantity) - records.Sum(r => r.Consumption);

                    if (currentStock <= chemical.MinimumStock)
                    {
                        chemicalsToNotify.Add((chemical, currentStock));
                    }
                }
                else
                {
                    _logger.LogInformation($"No records found for chemical {chemical.ChemicalName}. Skipping email notification.");
                }
            }

            _logger.LogInformation($"Found {chemicalsToNotify.Count} chemicals with minimum stock.");

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
            message.Cc.Add(MailboxAddress.Parse("NOVA.ASTUTI@infineon.com"));

            message.Subject = $"Minimum Stock Alert: {chemical.ChemicalName}";
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
            <h4>Dear All,</h4>
            <p>This alert notifies you that,</p>
            <p>Chemical Name : {chemical.ChemicalName},<br>Area : {chemical.Area.Name}.<br> Current Stock: {currentStock}. <br>Minimum Stock: {chemical.MinimumStock}</p>
            <p>Has reached the minimum stock number. <br> Prompt action to replenish the stock is required to ensure business continuity and efficiency. </p>
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

                _logger.LogInformation($"Minimum stock email sent for chemical {chemical.ChemicalName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send minimum stock email for chemical {chemical.ChemicalName}");
            }
        }
    }
}