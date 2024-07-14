using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChemicalProject.Data;
using ChemicalProject.Models;
using ChemicalProject.Helper;
using Microsoft.AspNetCore.Authorization;
using MimeKit;
using MailKit.Net.Smtp;


namespace ChemicalProject.Controllers
{
    [Authorize(Roles = "UserAdmin")]
    public class ApprovalESHController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserAreaService _userAreaService;

        public ApprovalESHController(ApplicationDbContext context, UserAreaService userAreaService)
        {
            _context = context;
            _userAreaService = userAreaService;

        }

        // GET: Chemical_FALab
        public IActionResult Index()
        {
            return View();
        }

        //API ENDPOINT
        [HttpGet]
        public IActionResult GetData()
        {
            var employees = _context.Chemicals
                .Where(g => g.StatusManager == true)
                .Where(g => g.StatusESH == null)
                .Select(g => new
                {
                    id = g.Id,
                    Badge = g.Badge,
                    AreaName = g.Area.Name,
                    ChemicalName = g.ChemicalName,
                    Brand = g.Brand,
                    Packaging = g.Packaging,
                    Unit = g.Unit,
                    MinimumStock = g.MinimumStock,
                    Price = g.Price,
                    Justify = g.Justify,
                    requestDate = g.RequestDate.HasValue ? g.RequestDate.Value.ToString("dd MMM yyyy HH:mm") : null,
                    StatusManager = g.StatusManager,
                    RemarkManager = g.RemarkManager,
                    approvalDateManager = g.ApprovalDateManager.HasValue ? g.ApprovalDateManager.Value.ToString("dd MMM yyyy HH:mm") : null,
                    StatusESH = g.StatusESH,
                    RemarkESH = g.RemarkESH,
                    approvalDateESH = g.ApprovalDateESH.HasValue ? g.ApprovalDateESH.Value.ToString("dd MMM yyyy HH:mm") : null,
                }).ToList();

            return Json(new { rows = employees });
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int? id, string remark)
        {
            var chemical = await _context.Chemicals.FindAsync(id);

            if (chemical == null)
            {
                return NotFound();
            }

            chemical.StatusESH = true;
            chemical.RemarkESH = remark;
            chemical.ApprovalDateESH = DateTime.Now;
            _context.Update(chemical);
            await _context.SaveChangesAsync();

            // Get UserArea's email with Area included
            var userArea = await _context.UserAreas
                .Include(ua => ua.Area)
                .FirstOrDefaultAsync(u => u.UserName == chemical.Username);

            // Get UserManager's email with Area included
            var userManager = await _context.UserManagers
                .Include(um => um.Area)
                .FirstOrDefaultAsync(um => um.AreaId == chemical.AreaId);

            await SendEmailNotification(chemical, true, userArea);

            return Json(new { success = true, message = "Chemical approved successfully!!!!" });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int? id, string remark)
        {
            var chemical = await _context.Chemicals.FindAsync(id);
            if (chemical == null)
            {
                return NotFound();
            }

            chemical.StatusESH = false;
            chemical.RemarkESH = remark;
            chemical.ApprovalDateESH = DateTime.Now;
            _context.Update(chemical);
            await _context.SaveChangesAsync();

            // Get UserArea's email with Area included
            var userArea = await _context.UserAreas
                .Include(ua => ua.Area)
                .FirstOrDefaultAsync(u => u.UserName == chemical.Username);

            await SendEmailNotification(chemical, false, userArea);

            return Json(new { success = true, message = "Chemical rejected successfully!!!!" });
        }

        private async Task SendEmailNotification(Chemical_FALab chemical, bool approved, UserArea userArea)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ESH Notification", "bth-esh@infineon.com"));

            // Add UserArea to recipients
            if (userArea != null)
            {
                message.To.Add(new MailboxAddress("Recipient", userArea.EmailUser));
                message.To.Add(new MailboxAddress("Recipient", userArea.EmailManager));
            }

            // Add admin ESH email to CC
            message.Cc.Add(new MailboxAddress("Admin ESH", "Andas.Puranda@infineon.com"));

            string approvalStatus = approved ? "approved" : "rejected";
            message.Subject = $"Chemical Request {approvalStatus} by ESH: {chemical.ChemicalName}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<p>Dear All,</p>" +
                                   $"<p>The chemical request for {chemical.ChemicalName} has been {approvalStatus} by ESH.</p>" +
                                   $"<p>Remark: {chemical.RemarkESH}</p>" +
                                   $"<p>Justify: {chemical.Justify}</p>" +
                                   $"<p>Regards,<br>ESH Notification System</p>";

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("mailrelay-internal.infineon.com", 25, false);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        private bool Chemical_FALabExists(int id)
        {
            return _context.Chemicals.Any(e => e.Id == id);
        }
    }
}
