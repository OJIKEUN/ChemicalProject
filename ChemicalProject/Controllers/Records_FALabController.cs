using ChemicalProject.Data;
using ChemicalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ChemicalProject.Controllers
{
    public class Records_FALabController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Records_FALabController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET INDEX
        public IActionResult Index(int id)
        {
            var chemicals = _context.Chemicals.FirstOrDefault(c => c.Id == id);
            if (chemicals == null)
            {
                return NotFound();
            }

            var records = _context.Records
                .Where(r => r.ChemicalId == id)
                .Include(r => r.Chemical_FALab)
                .ToList();

            var currentStock = records.Sum(r => r.ReceivedQuantity) - records.Sum(r => r.Consumption);

            ViewBag.ChemicalId = id;
            ViewBag.ChemicalName = chemicals.ChemicalName;
            ViewBag.CurrentStock = currentStock;

            return View();
        }

        [HttpGet]
        [Authorize(Policy = "AreaPolicy")]
        public IActionResult GetData(int id)
        {
            var userArea = User.FindFirst("Area")?.Value;
            var chemical = _context.Chemicals.Include(c => c.Area).FirstOrDefault(c => c.Id == id && c.Area.Name == userArea);
            if (chemical == null)
            {
                return Forbid();
            }
            var records = _context.Records
                .Where(r => r.ChemicalId == id)
                .Include(r => r.Chemical_FALab)
                .OrderBy(r => r.Id)
                .ToList();

            int currentStock = 0;
            var data = records
                .Select((r, index) =>
                {
                    currentStock += r.ReceivedQuantity - r.Consumption;

                    return new
                    {
                        id = r.Id,
                        badge = r.Badge,
                        chemicalName = r.Chemical_FALab.ChemicalName,
                        receivedQuantity = r.ReceivedQuantity,
                        consumption = r.Consumption,
                        currentStock = currentStock,
                        justify = r.Justify,
                        recordDate = r.RecordDate.HasValue ? r.RecordDate.Value.ToString("dd MMM yyyy HH:mm") : null,
                        receivedDate = r.ReceivedDate.HasValue ? r.ReceivedDate.Value.ToString("dd MMM yyyy HH:mm") : null,
                        expiredDate = r.ExpiredDate.HasValue ? r.ExpiredDate.Value.ToString("dd MMM yyyy HH:mm") : null
                    };
                })
                .ToList();

            return Json(new { rows = data });
        }

        // GET: Records_FALab/Create/5
        public IActionResult Create(int id)
        {
            var chemical = _context.Chemicals.FirstOrDefault(c => c.Id == id);

            if (chemical == null)
            {
                return NotFound();
            }

            var records = new Records_FALab
            {
                ChemicalId = id
            };

            ViewBag.ChemicalId = id;
            ViewBag.ChemicalName = chemical.ChemicalName;

            return View(records);
        }

        // POST: Records_FALab/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int chemicalId, [Bind("Badge,ReceivedQuantity,Consumption,Justify,RecordDate,ReceivedDate,ExpiredDate")] Records_FALab records)
        {
            var userArea = User.FindFirst("Area")?.Value;
            var chemical = _context.Chemicals.Include(c => c.Area).FirstOrDefault(c => c.Id == chemicalId && c.Area.Name == userArea);
            if (chemical == null)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                records.ChemicalId = chemicalId;
                records.RecordDate = DateTime.Now;
                records.ReceivedDate = DateTime.Now;
                records.ExpiredDate = DateTime.Now.AddMonths(1);

                _context.Add(records);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Record has been created successfully.";
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

            var records = await _context.Records
                .Include(r => r.Chemical_FALab)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (records == null)
            {
                return NotFound();
            }

            ViewData["ChemicalId"] = new SelectList(_context.Chemicals, "Id", "ChemicalName", records.ChemicalId);
            return View(records);
        }

        // POST: Records_FALab/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AreaPolicy")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChemicalId,Badge,ReceivedQuantity,Consumption,Justify,RecordDate,ReceivedDate,ExpiredDate")] Records_FALab records_FALab)
        {
            if (id != records_FALab.Id)
            {
                return NotFound();
            }

            var userArea = User.FindFirst("Area")?.Value;
            var chemical = await _context.Chemicals.Include(c => c.Area).FirstOrDefaultAsync(c => c.Id == records_FALab.ChemicalId);

            if (ModelState.IsValid && chemical != null && chemical.Area.Name == userArea)
            {
                try
                {
                    _context.Update(records_FALab);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Record has been updated successfully.";
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
                var chemicals = await _context.Chemicals.FirstOrDefaultAsync(c => c.Id == records_FALab.ChemicalId);
                if (chemicals == null)
                {
                    return NotFound();
                }

                // Menyimpan informasi ChemicalId dan ChemicalName ke ViewBag
                ViewBag.ChemicalId = records_FALab.ChemicalId;
                ViewBag.ChemicalName = chemicals.ChemicalName;

                return RedirectToAction(nameof(Index), new { id = records_FALab.ChemicalId });
            }

            ViewData["ChemicalId"] = new SelectList(_context.Chemicals, "Id", "ChemicalName", records_FALab.ChemicalId);
            return View(records_FALab);
        }

        private bool Records_FALabExists(int id)
        {
            return _context.Records.Any(e => e.Id == id);
        }

        // POST: Records_FALab/DeleteConfirmed/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, int chemicalId)
        {
            var records_FALab = await _context.Records.FindAsync(id);
            if (records_FALab != null)
            {
                _context.Records.Remove(records_FALab);
                await _context.SaveChangesAsync();
                return Json(new { success = true }); // Mengembalikan objek JSON dengan success = true
            }

            return Json(new { success = false }); // Mengembalikan objek JSON dengan success = false jika record tidak ditemukan
        }
    }
}