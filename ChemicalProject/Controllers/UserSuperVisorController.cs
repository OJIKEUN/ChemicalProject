using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChemicalProject.Data;
using ChemicalProject.Models;
using ChemicalProject.Helper;

namespace ChemicalProject.Controllers
{
    [Authorize(Roles = "UserAdmin")]
    public class UserSuperVisorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserAreaService _userAreaService;

        public UserSuperVisorController(ApplicationDbContext context, UserAreaService userAreaService)
        {
            _context = context;
            _userAreaService = userAreaService;
        }

        // GET: UserSuperVisor
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserSuperVisors.Include(u => u.Area);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var UserSuperVisors = _context.UserSuperVisors
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    userName = u.UserName,
                    areaName = u.Area.Name
                })
                .ToList();

            return Json(new { rows = UserSuperVisors });
        }

        // GET: UserSuperVisor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userSuperVisor = await _context.UserSuperVisors
                .Include(u => u.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userSuperVisor == null)
            {
                return NotFound();
            }

            return View(userSuperVisor);
        }

        // GET: UserSuperVisor/Create
        public IActionResult Create()
        {
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name");
            return View();
        }

        // POST: UserSuperVisor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AreaId,Name,UserName")] UserSuperVisor userSuperVisor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userSuperVisor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name", userSuperVisor.AreaId);
            return View(userSuperVisor);
        }


        // GET: UserSuperVisor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userSuperVisor = await _context.UserSuperVisors.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (userSuperVisor == null)
            {
                return NotFound();
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name", userSuperVisor.AreaId);
            return View(userSuperVisor);
        }

        // POST: UserSuperVisor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AreaId,Name,UserName")] UserSuperVisor userSuperVisor)
        {
            if (id != userSuperVisor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUserSuperVisor = await _context.UserSuperVisors.FindAsync(id);
                    if (existingUserSuperVisor == null)
                    {
                        return NotFound();
                    }

                    existingUserSuperVisor.AreaId = userSuperVisor.AreaId;
                    existingUserSuperVisor.Name = userSuperVisor.Name;
                    existingUserSuperVisor.UserName = userSuperVisor.UserName;

                    _context.Update(existingUserSuperVisor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserSuperVisorExists(userSuperVisor.Id))
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
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name", userSuperVisor.AreaId);
            return View(userSuperVisor);
        }

        // POST: UserSuperVisor/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userSuperVisor = await _context.UserSuperVisors.FindAsync(id);
            if (userSuperVisor != null)
            {
                _context.UserSuperVisors.Remove(userSuperVisor);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        private bool UserSuperVisorExists(int id)
        {
            return _context.UserSuperVisors.Any(e => e.Id == id);
        }
    }
}
