using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChemicalProject.Data;
using ChemicalProject.Models;

namespace ChemicalProject.Controllers
{
    [Authorize(Roles = "UserAdmin")]
    public class AreaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AreaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Area
        public async Task<IActionResult> Index()
        {
            return View(await _context.Areas.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var Areas = _context.Areas
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                })
                .ToList();

            return Json(new { rows = Areas });
        }

        // GET: Area/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // GET: Area/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Area/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Area area)
        {
            if (ModelState.IsValid)
            {
                _context.Add(area);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Area has been created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(area);
        }

        // GET: Area/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }
            return View(area);
        }

        // POST: Area/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Area area)
        {
            if (id != area.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(area);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Area has been updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaExists(area.Id))
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
            return View(area);
        }

        // POST: Area/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area != null)
            {
                _context.Areas.Remove(area);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        private bool AreaExists(int id)
        {
            return _context.Areas.Any(e => e.Id == id);
        }
    }
}
