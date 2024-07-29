using ChemicalProject.Data;
using ChemicalProject.Helper;
using ChemicalProject.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;


namespace ChemicalProject.Controllers
{
    [Authorize(Roles = "UserAdmin,UserManager,UserSuperVisor,UserArea")]
    public class ChemicalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserAreaService _userAreaService;
        public ChemicalController(ApplicationDbContext context, UserAreaService userAreaService)
        {
            _context = context;
            _userAreaService = userAreaService;
        }

        [Authorize(Roles = "UserAdmin,UserManager,UserSuperVisor,UserArea")]
        public IActionResult Index(int? areaId)
        {
            ViewBag.AreaId = areaId;
            return View();
        }

        //API ENDPOINT

        [HttpGet]
        [Authorize(Roles = "UserAdmin,UserManager,UserSuperVisor,UserArea")]
        public async Task<IActionResult> GetData(int? areaId)
        {
            var userAreaIds = await _userAreaService.GetUserAreaIdsAsync(User);

            IQueryable<Chemical_FALab> query = _context.Chemicals
                .Where(c => c.StatusManager != false && c.StatusESH != false);

            if (areaId.HasValue)
            {
                query = query.Where(c => c.AreaId == areaId.Value);
            }
            else if (!User.IsInRole("UserAdmin") && !User.IsInRole("UserManager") && userAreaIds.Any())
            {
                query = query.Where(c => userAreaIds.Contains(c.AreaId));
            }

            var chemicals = await query
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    badge = c.Badge,
                    chemicalName = c.ChemicalName,
                    brand = c.Brand,
                    packaging = c.Packaging,
                    unit = c.Unit,
                    minimumStock = c.MinimumStock,
                    price = c.Price,
                    costCentre = c.CostCentre,
                    justify = c.Justify,
                    requestDate = c.RequestDate.HasValue ? c.RequestDate.Value.ToString("dd MMM yyyy HH:mm") : null,
                    statusManager = c.StatusManager,
                    remarkManager = c.RemarkManager,
                    approvalDateManager = c.ApprovalDateManager.HasValue ? c.ApprovalDateManager.Value.ToString("dd MMM yyyy HH:mm") : null,
                    statusESH = c.StatusESH,
                    remarkESH = c.RemarkESH,
                    approvalDateESH = c.ApprovalDateESH.HasValue ? c.ApprovalDateESH.Value.ToString("dd MMM yyyy HH:mm") : null,
                    areaName = c.Area.Name
                })
                .ToListAsync();

            return Json(new { rows = chemicals });
        }
    

    // GET: Chemical_FALab/Create
    public IActionResult Create()
        {
            ViewBag.AreaId = new SelectList(_context.Areas, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Badge,ChemicalName,Brand,Packaging,Unit,MinimumStock,CostCentre,Justify,Status,RequestDate,AreaId")] Chemical_FALab chemical_FALab)
        {
            if (ModelState.IsValid)
            {
                chemical_FALab.RequestDate = DateTime.Now;
                chemical_FALab.Username = User.Identity.Name;

                _context.Add(chemical_FALab);
                await _context.SaveChangesAsync();

                var userArea = _context.UserAreas.FirstOrDefault(ua => ua.UserName == chemical_FALab.Username);
                if (userArea != null)
                {
                    SendEmailNotification(userArea.EmailUser, userArea.EmailManager, chemical_FALab.Username);
                }

                TempData["SuccessMessage"] = "Chemical has been created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.AreaId = new SelectList(_context.Areas, "Id", "Name");
            return View(chemical_FALab);
        }
        private void SendEmailNotification(string emailUser, string emailManager, string userName)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ESH Notification", "bth-esh@infineon.com"));
            message.To.Add(new MailboxAddress("Recipient", emailManager));
            message.Cc.Add(new MailboxAddress("CC Recipient", emailUser));
            message.Subject = $"New Chemical Request from {userName}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<p>Dear All,</p><p>{userName} has submitted a new chemical request. Please review and take necessary actions.</p><p>Regards,<br>ESH Notification System</p>";

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("mailrelay-internal.infineon.com", 25, false);
                client.Send(message);
                client.Disconnect(true);
            }
        }
        // GET: Chemical_FALab/Edit/5
        [Authorize(Roles = "UserAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemical_FALab = await _context.Chemicals.Include(c => c.Area).FirstOrDefaultAsync(c => c.Id == id);
            if (chemical_FALab == null)
            {
                return NotFound();
            }
            ViewBag.AreaId = new SelectList(_context.Areas, "Id", "Name", chemical_FALab.AreaId);
            return View(chemical_FALab);
        }

        // POST: Chemical_FALab/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Badge,ChemicalName,Brand,Packaging,MinimumStock,Unit,CostCentre,Justify,AreaId")] Chemical_FALab chemical_FALab)
        {
            if (id != chemical_FALab.Id)
            {
                return NotFound();
            }
            var existingRecord = await _context.Chemicals.FindAsync(id);
            if (existingRecord == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingRecord.Name = chemical_FALab.Name;
                    existingRecord.Badge = chemical_FALab.Badge;
                    existingRecord.AreaId = chemical_FALab.AreaId;
                    existingRecord.ChemicalName = chemical_FALab.ChemicalName;
                    existingRecord.Brand = chemical_FALab.Brand;
                    existingRecord.Packaging = chemical_FALab.Packaging;
                    existingRecord.Unit = chemical_FALab.Unit;
                    existingRecord.MinimumStock = chemical_FALab.MinimumStock;
                    existingRecord.CostCentre = chemical_FALab.CostCentre;
                    existingRecord.Justify = chemical_FALab.Justify;
                    /*existingRecord.RequestDate = chemical_FALab.RequestDate;*/

                    _context.Update(existingRecord);
                    TempData["SuccessMessage"] = "Chemical has been edited successfully.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Chemical_FALabExists(existingRecord.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(chemical_FALab);
        }


        // GET: Chemical_FALab/Delete/5
        [Authorize(Roles = "UserAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemical_FALab = await _context.Chemicals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chemical_FALab == null)
            {
                return NotFound();
            }

            return View(chemical_FALab);
        }

        // POST: Chemical_FALab/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chemical_FALab = await _context.Chemicals.FindAsync(id);
            if (chemical_FALab != null)
            {
                _context.Chemicals.Remove(chemical_FALab);
                TempData["SuccessMessage"] = "Chemical has been deleted successfully.";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        //-----------//
        private bool Chemical_FALabExists(int id)
        {
            return _context.Chemicals.Any(e => e.Id == id);
        }

        public IActionResult ApprovalList()
        {
            var chemicalsToApprove = _context.Chemicals.Where(c => c.StatusManager == null);
            return View(chemicalsToApprove);
        }
    }
}



/*if (!User.IsInRole("UserAdmin") && !User.IsInRole("UserManager") && !User.IsInRole("UserSuperVisor") && !User.IsInRole("UserArea"))
{
    return View("~/Views/Shared/AccessDenied.cshtml");
}*/