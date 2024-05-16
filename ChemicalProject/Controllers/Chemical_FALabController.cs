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


namespace ChemicalProject.Controllers
{
    public class Chemical_FALabController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Chemical_FALabController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // GET: Chemical_FALab
        public IActionResult Index()
        {
            return View();
        }
        // untuk dapet data di table
        //API ENDPOINT
        [HttpGet]
        public IActionResult GetData()
        {
            var Chemicals = _context.Chemicals
                .Select(c => new
                {
                    id = c.Id,
                    badge = c.Badge,
                    chemicalName = c.ChemicalName,
                    brand = c.Brand,
                    packaging = c.Packaging,
                    unit = c.Unit,
                    minimumStock = c.MinimumStock,
                    price = c.Price,
                    justify = c.Justify,
                    requestDate = c.RequestDate.HasValue ? c.RequestDate.Value.ToString("dd MMM yyyy HH:mm") : null,
                    statusManager = c.StatusManager,
                    remarkManager = c.RemarkManager,
                    approvalDateManager = c.ApprovalDateManager.HasValue ? c.ApprovalDateManager.Value.ToString("dd MMM yyyy HH:mm") : null,
                    statusESH = c.StatusESH,
                    remarkESH = c.RemarkESH,
                    approvalDateESH = c.ApprovalDateESH.HasValue ? c.ApprovalDateESH.Value.ToString("dd MMM yyyy HH:mm") : null,
                    areaName = c.Area.Name
                }).ToList();

            return Json(new { rows = Chemicals });
        }

        // GET: Chemical_FALab/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Chemical_FALab/Create
        public IActionResult Create()
        {
            ViewBag.AreaId = new SelectList(_context.Areas, "Id", "Name");
            return View();
        }

        // POST: Chemical_FALab/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Badge,ChemicalName,Brand,Packaging,Unit,MinimumStock,price,Justify,Status,RequestDate,AreaId")] Chemical_FALab chemical_FALab)
        {
            if (ModelState.IsValid)
            {
                chemical_FALab.RequestDate = DateTime.Now;
                
                _context.Add(chemical_FALab);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Chemical has been created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(chemical_FALab);
        }

        // GET: Chemical_FALab/Edit/5
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Badge,ChemicalName,Brand,Packaging,MinimumStock,Unit,price,Justify,RequestDate,AreaId")] Chemical_FALab chemical_FALab)
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
                    existingRecord.Badge = chemical_FALab.Badge;
                    existingRecord.AreaId = chemical_FALab.AreaId;
                    existingRecord.ChemicalName = chemical_FALab.ChemicalName;
                    existingRecord.Brand = chemical_FALab.Brand;
                    existingRecord.Packaging = chemical_FALab.Packaging;
                    existingRecord.Unit = chemical_FALab.Unit;
                    existingRecord.MinimumStock = chemical_FALab.MinimumStock;
                    existingRecord.Price = chemical_FALab.Price;
                    existingRecord.Justify = chemical_FALab.Justify;
                    existingRecord.RequestDate = chemical_FALab.RequestDate;

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
