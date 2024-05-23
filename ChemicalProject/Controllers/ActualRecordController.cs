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

                // Mengambil currentStock dari tabel Records
                var records = await _context.Records
                    .Where(r => r.ChemicalId == id)
                    .ToListAsync();
                var currentStock = records.Sum(r => r.ReceivedQuantity) - records.Sum(r => r.Consumption);

                ViewBag.ChemicalId = id;
                ViewBag.ChemicalName = chemical.ChemicalName;
                ViewBag.CurrentStock = currentStock;
                ViewBag.AreaId = chemical.AreaId;

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

            // Mengambil ActualRecords
            var records = _context.ActualRecords
                .Where(r => r.ChemicalId == id)
                .Include(r => r.Chemical_FALab)
                .OrderBy(r => r.Id) // Mengurutkan berdasarkan Id untuk menjaga urutan kronologis
                .ToList();

            // Mengambil currentStock dari tabel Records
            var recordsSummary = _context.Records
                .Where(r => r.ChemicalId == id)
                .ToList();
            var currentStock = recordsSummary.Sum(r => r.ReceivedQuantity) - recordsSummary.Sum(r => r.Consumption);

            // Memetakan data actual records, gunakan currentStock sebagai satu nilai konstan
            var data = records
                .Select(r => new
                {
                    id = r.Id,
                    badge = r.Badge,
                    areaId = r.Chemical_FALab.AreaId,
                    chemicalName = r.Chemical_FALab.ChemicalName,
                    description = r.Description,
                    date = r.Date.ToString("dd MMM yyyy"),
                    currentStock = currentStock
                })
                .ToList();

            return Json(new { rows = data });
        }




        // GET: ActualRecord/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: ActualRecord/Create
        public IActionResult Create()
        {
            ViewData["ChemicalId"] = new SelectList(_context.Chemicals, "Id", "Id");
            return View();
        }

        // POST: ActualRecord/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Badge,Description,Date,ChemicalId")] ActualRecord actualRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actualRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChemicalId"] = new SelectList(_context.Chemicals, "Id", "Id", actualRecord.ChemicalId);
            return View(actualRecord);
        }

        // GET: ActualRecord/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actualRecord = await _context.ActualRecords.FindAsync(id);
            if (actualRecord == null)
            {
                return NotFound();
            }
            ViewData["ChemicalId"] = new SelectList(_context.Chemicals, "Id", "Id", actualRecord.ChemicalId);
            return View(actualRecord);
        }

        // POST: ActualRecord/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    _context.Update(actualRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActualRecordExists(actualRecord.Id))
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
            ViewData["ChemicalId"] = new SelectList(_context.Chemicals, "Id", "Id", actualRecord.ChemicalId);
            return View(actualRecord);
        }

        // GET: ActualRecord/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: ActualRecord/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actualRecord = await _context.ActualRecords.FindAsync(id);
            if (actualRecord != null)
            {
                _context.ActualRecords.Remove(actualRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActualRecordExists(int id)
        {
            return _context.ActualRecords.Any(e => e.Id == id);
        }
    }
}
