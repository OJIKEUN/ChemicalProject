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
    [Authorize(Roles = "UserAdmin")]
    public class UserManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserAreaService _userAreaService;

        public UserManagerController(ApplicationDbContext context, UserAreaService userAreaService)
        {
            _context = context;
            _userAreaService = userAreaService;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserManagers.Include(u => u.Area);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var UserManagers = _context.UserManagers    
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    userName = u.UserName,
                    areaName = u.Area.Name
                })
                .ToList();

            return Json(new { rows = UserManagers });
        }

        // GET: UserManager/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userManager = await _context.UserManagers
                .Include(u => u.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userManager == null)
            {
                return NotFound();
            }

            return View(userManager);
        }

        // GET: UserManager/Create
        public IActionResult Create()
        {
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name");
            return View();
        }

        // POST: UserManager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AreaId,Name,UserName")] UserManager userManager)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userManager);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Manager has been created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name", userManager.AreaId);
            return View(userManager);
        }

        // GET: UserManager/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userManager = await _context.UserManagers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (userManager == null)
            {
                return NotFound();
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name", userManager.AreaId);
            return View(userManager);
        }


        // POST: UserManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AreaId,Name,UserName")] UserManager userManager)
        {
            if (id != userManager.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing data before updating
                    var existingUserManager = await _context.UserManagers.FindAsync(id);

                    if (existingUserManager == null)
                    {
                        return NotFound();
                    }

                    // Update the existingUserManager properties
                    existingUserManager.AreaId = userManager.AreaId;
                    existingUserManager.Name = userManager.Name;
                    existingUserManager.UserName = userManager.UserName;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Manager has been updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserManagerExists(userManager.Id))
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
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name", userManager.AreaId);
            return View(userManager);
        }

        private bool UserManagerExists(int id)
        {
            return _context.UserManagers.Any(e => e.Id == id);
        }

        // POST: UserManager/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userManager = await _context.UserManagers.FindAsync(id);
            if (userManager != null)
            {
                _context.UserManagers.Remove(userManager);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
    }
}
