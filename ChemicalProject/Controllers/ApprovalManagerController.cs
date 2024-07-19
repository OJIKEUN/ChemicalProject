using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChemicalProject.Data;
using ChemicalProject.Models;
using Microsoft.AspNetCore.Authorization;
using ChemicalProject.Helper;
using MimeKit;
using MailKit.Net.Smtp;


namespace ChemicalProject.Controllers
{
    public class ApprovalManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserAreaService _userAreaService;

        public ApprovalManagerController(ApplicationDbContext context, UserAreaService userAreaService)
        {
            _context = context;
            _userAreaService = userAreaService;
        }

        [Authorize(Roles = "UserAdmin,UserManager")]
        public async Task<IActionResult> Index()
        {
            var userAreaIds = await _userAreaService.GetUserAreaIdsAsync(User);
            var isUserAdmin = User.IsInRole("UserAdmin");

            if (isUserAdmin)
            {
                var allChemicals = await _context.Chemicals
                    .Where(c => c.StatusManager == null)
                    .ToListAsync();
                return View(allChemicals);
            }
            else
            {
                var isUserManager = User.IsInRole("UserManager");
                if (isUserManager)
                {
                    var currentUserAreaId = _context.UserManagers.FirstOrDefault(u => u.UserName == User.Identity.Name)?.AreaId;
                    var chemicals = await _context.Chemicals
                        .Where(c => c.AreaId == currentUserAreaId && c.StatusManager == null)
                        .ToListAsync();

                    return View(chemicals);
                }
                else
                {
                    return Forbid();
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var isUserAdmin = User.IsInRole("UserAdmin");

            if (isUserAdmin)
            {
                var chemicals = await _context.Chemicals
                    .Where(g => g.StatusManager == null)
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
                    }).ToListAsync();
                return Json(new { rows = chemicals });
            }
            else
            {
                var isUserManager = User.IsInRole("UserManager");
                if (isUserManager)
                {
                    var currentUserAreaId = _context.UserManagers.FirstOrDefault(u => u.UserName == User.Identity.Name)?.AreaId;
                    var chemicals = await _context.Chemicals
                        .Where(g => g.AreaId == currentUserAreaId && g.StatusManager == null)
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
                        }).ToListAsync();
                    return Json(new { rows = chemicals });
                }
                else
                {
                    return Forbid();
                }
            }
        }
    

    [HttpPost]
        public async Task<IActionResult> Approve(int? id, string remark)
        {
            var chemical = await _context.Chemicals.FindAsync(id);
            if (chemical == null)
            {
                return NotFound();
            }
            chemical.StatusManager = true;
            chemical.RemarkManager = remark;
            chemical.ApprovalDateManager = DateTime.Now;
            _context.Update(chemical);
            await _context.SaveChangesAsync();
            // Get UserArea's email with Area included
            var userArea = await _context.UserAreas
            .Include(ua => ua.Area)
            .FirstOrDefaultAsync(u => u.UserName == chemical.Username);
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
            chemical.StatusManager = false;
            chemical.RemarkManager = remark;
            chemical.ApprovalDateManager = DateTime.Now;
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
            if (userArea != null)
            {
                message.To.Add(new MailboxAddress("Recipient", userArea.EmailUser));
                message.Cc.Add(new MailboxAddress("CC Recipient", userArea.EmailManager));
            }
            // Add admin ESH email to CC
            message.Cc.Add(new MailboxAddress("Admin ESH", "Andas.Puranda@infineon.com"));
            string approvalStatus = approved ? "approved" : "rejected";
            message.Subject = $"Chemical Request {approvalStatus} by Manager: {chemical.ChemicalName}";
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<p>Dear All,</p>" +
            $"<p>The chemical request for {chemical.ChemicalName} has been {approvalStatus} by the Manager.</p>" +
            $"<p>Remark: {chemical.RemarkManager}</p>" +
            $"<p>Justify: {chemical.Justify}</p>" +
            $"<p>Regards,<br>ESH Notification System</p>";
            message.Body = bodyBuilder.ToMessageBody();
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("mailrelay-internal.infineon.com", 25, false);
                    client.Send(message);
                    Console.WriteLine("Email sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                    // You can also log the error to a file or database
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }

        private bool Chemical_FALabExists(int id)
        {
            return _context.Chemicals.Any(e => e.Id == id);
        }
    }
}