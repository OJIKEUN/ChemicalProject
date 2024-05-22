using ChemicalProject.Data;
using ChemicalProject.Helper;
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
        private readonly UserAreaService _userAreaService;

        public Records_FALabController(ApplicationDbContext context, UserAreaService userAreaService)
        {
            _context = context;
            _userAreaService = userAreaService;
        }

        //GET INDEX
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
                var records = await _context.Records
                    .Where(r => r.ChemicalId == id)
                    .Include(r => r.Chemical_FALab)
                    .ToListAsync();

                var currentStock = records.Sum(r => r.ReceivedQuantity) - records.Sum(r => r.Consumption);

                ViewBag.ChemicalId = id;
                ViewBag.ChemicalName = chemical.ChemicalName;
                ViewBag.CurrentStock = currentStock;

                return View();
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
            var records = _context.Records
                .Where(r => r.ChemicalId == id)
                .Include(r => r.Chemical_FALab)
                .OrderBy(r => r.Id) // Mengurutkan berdasarkan Id untuk menjaga urutan kronologis
                .ToList();

            int currentStock = 0; // Inisialisasi nilai stok saat ini
            var data = records
                .Select((r, index) =>
                {
                    // Kalkulasi stok saat ini secara kumulatif
                    currentStock += r.ReceivedQuantity - r.Consumption;

                    return new
                    {
                        id = r.Id,
                        badge = r.Badge,
                        chemicalName = r.Chemical_FALab.ChemicalName,
                        receivedQuantity = r.ReceivedQuantity,
                        consumption = r.Consumption,
                        currentStock = currentStock, // Menggunakan nilai stok saat ini yang telah dikalkulasi
                        justify = r.Justify,
                        recordDate = r.RecordDate.HasValue ? r.RecordDate.Value.ToString("dd MMM yyyy") : null,
                        receivedDate = r.ReceivedDate.HasValue ? r.ReceivedDate.Value.ToString("dd MMM yyyy") : null,
                        expiredDate = r.ExpiredDate.HasValue ? r.ExpiredDate.Value.ToString("dd MMM yyyy") : null
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
        // POST: Records_FALab/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int chemicalId, [Bind("Badge,ReceivedQuantity,Consumption,Justify,RecordDate,ReceivedDate,ExpiredDate")] Records_FALab records)
        {

            if (ModelState.IsValid)
            {
                records.ChemicalId = chemicalId;
                records.RecordDate = DateTime.Now;
                records.ReceivedDate = DateTime.Now;
                records.ExpiredDate = DateTime.Now.AddMonths(6);

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