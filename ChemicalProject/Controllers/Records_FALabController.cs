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
    public class Records_FALabController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Records_FALabController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index(int id)
        {
            var chemical = _context.Chemicals.FirstOrDefault(c => c.Id == id);
            if (chemical == null)
            {
                return NotFound();
            }

            var records = _context.Records
                .Where(r => r.ChemicalId == id)
                .Include(r => r.Chemical_FALab)
                .ToList();

            var currentStock = records.Sum(r => r.ReceivedQuantity) - records.Sum(r => r.Consumption);

            ViewBag.ChemicalId = id;
            ViewBag.ChemicalName = chemical.ChemicalName;
            ViewBag.CurrentStock = currentStock;

            return View();
        }

        [HttpGet]
        public IActionResult GetData(int id)
        {
            var chemical = _context.Chemicals.FirstOrDefault(c => c.Id == id);
            var records = _context.Records
                .Where(r => r.ChemicalId == id)
                .Include(r => r.Chemical_FALab)
                .Select(r => new
                {
                    id = r.Id,
                    badge = r.Badge,
                    chemicalName = r.Chemical_FALab.ChemicalName,
                    receivedQuantity = r.ReceivedQuantity,
                    consumption = r.Consumption,
                    justify = r.Justify,
                    recordDate = r.RecordDate.HasValue ? r.RecordDate.Value.ToString("dd MMM yyyy") : null,
                    receivedDate = r.ReceivedDate.HasValue ? r.ReceivedDate.Value.ToString("dd MMM yyyy") : null,
                    expiredDate = r.ExpiredDate.HasValue ? r.ExpiredDate.Value.ToString("dd MM yyyy") : null
                })
                .ToList();

            return Json(new { rows = records });
        }


        //GET CREATE
        public IActionResult Create(int id)
        {
            var record = new Records_FALab
            {
                ChemicalId = id
            };

            ViewBag.ChemicalId = id;
            return View(record);
        }

        //POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int chemicalId, [Bind("Badge,ReceivedQuantity,Consumption,Justify,RecordDate,ReceivedDate,ExpiredDate")] Records_FALab records)
        {
            if (ModelState.IsValid)
            {
                records.ChemicalId = chemicalId;
                records.Badge = 123;
                records.RecordDate = DateTime.Now;
                records.ReceivedDate = DateTime.Now;
                records.ExpiredDate = DateTime.Now.AddMonths(6);

                _context.Add(records);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index), new { id = chemicalId });
            }

            return View(records);
        }

        // GET: Records_FALab/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var records_FALab = await _context.Records
                .Include(r => r.Chemical_FALab)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (records_FALab == null)
            {
                return NotFound();
            }

            ViewData["ChemicalId"] = new SelectList(_context.Chemicals, "Id", "ChemicalName", records_FALab.ChemicalId);
            return View(records_FALab);
        }

        // POST: Records_FALab/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChemicalId,Badge,ReceivedQuantity,Consumption,Justify,RecordDate,ReceivedDate,ExpiredDate")] Records_FALab records_FALab)
        {
            if (id != records_FALab.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(records_FALab);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Records_FALabExists(records_FALab.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Mendapatkan data Chemical_FALab berdasarkan ChemicalId yang diedit
                var chemical = await _context.Chemicals.FirstOrDefaultAsync(c => c.Id == records_FALab.ChemicalId);
                if (chemical == null)
                {
                    return NotFound();
                }

                // Menyimpan informasi ChemicalId dan ChemicalName ke ViewBag
                ViewBag.ChemicalId = records_FALab.ChemicalId;
                ViewBag.ChemicalName = chemical.ChemicalName;

                return RedirectToAction(nameof(Index), new { id = records_FALab.ChemicalId });
            }

            ViewData["ChemicalId"] = new SelectList(_context.Chemicals, "Id", "ChemicalName", records_FALab.ChemicalId);
            return View(records_FALab);
        }

        // GET: Records_FALab/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var records_FALab = await _context.Records
                .Include(r => r.Chemical_FALab)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (records_FALab == null)
            {
                return NotFound();
            }

            return View(records_FALab);
        }

        // POST: Records_FALab/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var records_FALab = await _context.Records.FindAsync(id);
            if (records_FALab != null)
            {
                _context.Records.Remove(records_FALab);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { chemicalId = records_FALab.ChemicalId });
        }

        private bool Records_FALabExists(int id)
        {
            return _context.Records.Any(e => e.Id == id);
        }
    }
}