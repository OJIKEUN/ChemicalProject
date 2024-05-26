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
    public class UserAreaController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserAreaService _userAreaService;

        public UserAreaController(ApplicationDbContext context, UserAreaService userAreaService)
        {
            _context = context;
            _userAreaService = userAreaService;
        }

        // GET: UserArea
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserAreas.Include(u => u.Area);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var UserAreas = _context.UserAreas
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    userName = u.UserName,
                    areaName = u.Area.Name,
                    EmailUser = u.EmailUser,
                    EmailManager = u.EmailManager
                })
                .ToList();

            return Json(new { rows = UserAreas });
        }
        // GET: UserArea/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userArea = await _context.UserAreas
                .Include(u => u.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userArea == null)
            {
                return NotFound();
            }

            return View(userArea);
        }

        // GET: UserArea/Create
        public IActionResult Create()
        {
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name");
            return View();
        }

        // POST: UserArea/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AreaId,Name,UserName,EmailUser,EmailManager")] UserArea userArea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userArea);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User Area has been created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name", userArea.AreaId);
            return View(userArea);
        }

        // GET: UserArea/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userArea = await _context.UserAreas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (userArea == null)
            {
                return NotFound();
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name", userArea.AreaId);
            return View(userArea);
        }

        // POST: UserArea/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AreaId,Name,UserName,EmailUser,EmailManager")] UserArea userArea)
        {
            if (id != userArea.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUserArea = await _context.UserAreas.FindAsync(id);
                    if (existingUserArea == null)
                    {
                        return NotFound();
                    }

                    existingUserArea.AreaId = userArea.AreaId;
                    existingUserArea.Name = userArea.Name;
                    existingUserArea.UserName = userArea.UserName;
                    existingUserArea.EmailUser = userArea.EmailUser;
                    existingUserArea.EmailManager = userArea.EmailManager;

                    _context.Update(existingUserArea);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "User Area has been updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAreaExists(userArea.Id))
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
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Name", userArea.AreaId);
            return View(userArea);
        }


        // POST: UserArea/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userArea = await _context.UserAreas.FindAsync(id);
            if (userArea != null)
            {
                _context.UserAreas.Remove(userArea);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        private bool UserAreaExists(int id)
        {
            return _context.UserAreas.Any(e => e.Id == id);
        }
    }
}
