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
using ChemicalProject.Helper;

namespace ChemicalProject.Controllers
{
    [Authorize(Roles = "UserAdmin")]
    public class UserAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserAreaService _userAreaService;

        public UserAdminController(ApplicationDbContext context, UserAreaService userAreaService)
        {
            _context = context;
            _userAreaService = userAreaService;
        }
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserAdmins.Include(u => u.Area);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var userAdmins = _context.UserAdmins
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    userName = u.UserName
                })
                .ToList();

            return Json(new { rows = userAdmins });
        }

        // GET: UserAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAdmin = await _context.UserAdmins
                .Include(u => u.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAdmin == null)
            {
                return NotFound();
            }

            return View(userAdmin);
        }

        // GET: UserAdmin/Create
        public IActionResult Create()
        {
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name");
            return View();
        }

        // POST: UserAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AreaId,Name,UserName")] UserAdmin userAdmin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userAdmin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name", userAdmin.AreaId);
            return View(userAdmin);
        }

        // GET: UserAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAdmin = await _context.UserAdmins.FindAsync(id);
            if (userAdmin == null)
            {
                return NotFound();
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name", userAdmin.AreaId);
            return View(userAdmin);
        }

        // POST: UserAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AreaId,Name,UserName")] UserAdmin userAdmin)
        {
            if (id != userAdmin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAdmin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAdminExists(userAdmin.Id))
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
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name", userAdmin.AreaId);
            return View(userAdmin);
        }

        // GET: UserAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAdmin = await _context.UserAdmins
                .Include(u => u.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAdmin == null)
            {
                return NotFound();
            }

            return View(userAdmin);
        }

        // POST: UserAdmin/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userAdmin = await _context.UserAdmins.FindAsync(id);
            if (userAdmin != null)
            {
                _context.UserAdmins.Remove(userAdmin);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        private bool UserAdminExists(int id)
        {
            return _context.UserAdmins.Any(e => e.Id == id);
        }
    }
}
