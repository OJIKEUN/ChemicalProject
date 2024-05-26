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

namespace ChemicalProject.Controllers
{
    
    public class ActualRecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserAreaService _userAreaService;

        public ActualRecordController(ApplicationDbContext context, UserAreaService userAreaService)
        {
            _context = context;
            _userAreaService = userAreaService;
        }

        [Authorize(Roles = "UserAdmin,UserManager,UserSuperVisor,UserArea")]
        public async Task<IActionResult> Index(int id)
        {
            var chemical = await _context.Chemicals.FirstOrDefaultAsync(c => c.Id == id);
            if (chemical == null)
            {
                return NotFound();
            }

            var userAreaId = await _userAreaService.GetUserAreaIdAsync(User);
            var isUserAdmin = userAreaId == null;

            if (isUserAdmin || userAreaId == chemical.AreaId)
            {
                var actualRecords = await _context.ActualRecords
                    .Where(r => r.ChemicalId == id)
                    .Include(r => r.Chemical_FALab)
                    .ToListAsync();

                var records = await _context.Records
                    .Where(r => r.ChemicalId == id)
                    .ToListAsync();
                var currentStock = records.Sum(r => r.ReceivedQuantity) - records.Sum(r => r.Consumption);

                ViewBag.ChemicalId = id;
                ViewBag.ChemicalName = chemical.ChemicalName;
                ViewBag.CurrentStock = currentStock;
                ViewBag.AreaId = chemical.AreaId;

                ViewBag.IsUserAdmin = User.IsInRole("UserAdmin");
                ViewBag.IsUserManager = User.IsInRole("UserManager");
                ViewBag.IsUserArea = User.IsInRole("UserArea");
                ViewBag.IsUserSupervisor = User.IsInRole("UserSuperVisor");

                return View(actualRecords);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        public IActionResult GetData(int id)
        {
            var chemical = _context.Chemicals.FirstOrDefault(c => c.Id == id);
            var records = _context.ActualRecords
                .Where(r => r.ChemicalId == id)
                .Include(r => r.Chemical_FALab)
                .OrderBy(r => r.Id) 
                .ToList();
            
            var data = records
                .Select(r => new
                {
                    id = r.Id,
                    badge = r.Badge,
                    areaId = r.Chemical_FALab.AreaId,
                    chemicalName = r.Chemical_FALab.ChemicalName,
                    description = r.Description,
                    date = r.Date.ToString("dd MMM yyyy HH:mm"),
                    currentStock = r.CurrentStock 
                })
                .ToList();

            return Json(new { rows = data });
        }

        // GET: ActualRecord/Create
        [Authorize(Roles = "UserAdmin,UserSuperVisor")]
        public IActionResult Create(int chemicalId)
        {
            var chemical = _context.Chemicals.FirstOrDefault(c => c.Id == chemicalId);
            if (chemical == null)
            {
                return NotFound();
            }

            ViewBag.ChemicalId = chemicalId;
            return View(new ActualRecord { ChemicalId = chemicalId });
        }

        // POST: ActualRecord/Create
        [Authorize(Roles = "UserAdmin,UserSuperVisor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Badge,Description,Date,ChemicalId")] ActualRecord actualRecord)
        {
            var chemical = await _context.Chemicals.FirstOrDefaultAsync(c => c.Id == actualRecord.ChemicalId);
            if (chemical == null)
            {
                ModelState.AddModelError("ChemicalId", "Invalid Chemical ID");
            }

            if (ModelState.IsValid)
            {
                var records = await _context.Records
                    .Where(r => r.ChemicalId == actualRecord.ChemicalId)
                    .ToListAsync();

                actualRecord.CurrentStock = records.Sum(r => r.ReceivedQuantity) - records.Sum(r => r.Consumption);
                _context.Add(actualRecord);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Record created successfully";
                return RedirectToAction(nameof(Index), new { id = actualRecord.ChemicalId });
            }
            ViewBag.ChemicalId = actualRecord.ChemicalId;
            return View(actualRecord);
        }

        // GET: ActualRecord/Edit/5
        [Authorize(Roles = "UserAdmin,UserSuperVisor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actualRecord = await _context.ActualRecords
                .Include(a => a.Chemical_FALab)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actualRecord == null)
            {
                return NotFound();
            }

            return View(actualRecord);
        }

        // POST: ActualRecord/Edit/5
        [Authorize(Roles = "UserAdmin,UserSuperVisor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Badge,Description,Date,ChemicalId")] ActualRecord actualRecord)
        {
            if (id != actualRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRecord = await _context.ActualRecords.FindAsync(id);

                    if (existingRecord == null)
                    {
                        return NotFound();
                    }
                    existingRecord.Badge = actualRecord.Badge;
                    existingRecord.Description = actualRecord.Description;
                    existingRecord.Date = actualRecord.Date;
                    existingRecord.ChemicalId = actualRecord.ChemicalId;

                    _context.Update(existingRecord);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Record update successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActualRecordExistsMethod(actualRecord.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = actualRecord.ChemicalId });
            }
            ViewData["ChemicalId"] = new SelectList(_context.Chemicals, "Id", "ChemicalName", actualRecord.ChemicalId);
            return View(actualRecord);
        }

        private bool ActualRecordExistsMethod(int id) 
        {
            return _context.ActualRecords.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actualRecord = await _context.ActualRecords.FindAsync(id);
            if (actualRecord != null)
            {
                _context.ActualRecords.Remove(actualRecord);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Record deleted successfully" });
            }

            return Json(new { success = false, message = "Record not found" });
        }

    }
}
