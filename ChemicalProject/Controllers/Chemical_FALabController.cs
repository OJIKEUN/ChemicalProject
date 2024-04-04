using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChemicalProject.Data;
using ChemicalProject.Models;


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
        public JsonResult GetData()
        {
            var chemical = _context.Chemicals
                .ToList();

            return Json(chemical);
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
            return View();
        }

        // POST: Chemical_FALab/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Badge,ChemicalName,Brand,Packaging,Unit,price,Justify,Status")] Chemical_FALab chemical_FALab)
        {
            if (ModelState.IsValid)
            {
                /*chemical_FALab.RequestDate = DateTime.Now;*/
                _context.Add(chemical_FALab);
                await _context.SaveChangesAsync();
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

            var chemical_FALab = await _context.Chemicals.FindAsync(id);
            if (chemical_FALab == null)
            {
                return NotFound();
            }
            return View(chemical_FALab);
        }

        // POST: Chemical_FALab/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Badge,ChemicalName,Brand,Packaging,Unit,price,Justify")] Chemical_FALab chemical_FALab)
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
                    existingRecord.ChemicalName = chemical_FALab.ChemicalName;
                    existingRecord.Brand = chemical_FALab.Brand;
                    existingRecord.Packaging = chemical_FALab.Packaging;
                    existingRecord.Unit = chemical_FALab.Unit;
                    existingRecord.price = chemical_FALab.price;
                    existingRecord.Justify = chemical_FALab.Justify;
                    _context.Update(existingRecord);
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

        [HttpPost]
        public async Task<IActionResult> Approve(int? id)
        {
            var chemical = await _context.Chemicals.FindAsync(id);

            if (chemical == null)
            {
                return NotFound();
            }

            chemical.Status = true;
            _context.Update(chemical);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Chemical approved successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int? id)
        {
            var chemical = await _context.Chemicals.FindAsync(id);

            if (chemical == null)
            {
                return NotFound();
            }

            chemical.Status = false;
            _context.Update(chemical);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Chemical rejected successfully!" });
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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Chemical_FALabExists(int id)
        {
            return _context.Chemicals.Any(e => e.Id == id);
        }

        public IActionResult ApprovalList()
        {
            var chemicalsToApprove = _context.Chemicals.Where(c => c.Status == null);
            return View(chemicalsToApprove);
        }


    }
}
