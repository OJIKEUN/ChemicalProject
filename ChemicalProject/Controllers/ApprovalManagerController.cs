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
    public class ApprovalManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApprovalManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Chemical_FALab
        public IActionResult Index()
        {
            return View();
        }

        //API ENDPOINT
        [HttpGet]
        public IActionResult GetData()
        {
            var employees = _context.Chemicals
                .Where(g => g.StatusManager == null)
                .Select(g => new
                {
                    id = g.Id,
                    Badge = g.Badge,
                    ChemicalName = g.ChemicalName,
                    Brand = g.Brand,
                    Packaging = g.Packaging,
                    Unit = g.Unit,
                    MinimumStock = g.MinimumStock,
                    Price = g.Price,
                    Justify = g.Justify,
                    requestDate = g.RequestDate.HasValue ? g.RequestDate.Value.ToString("dd MMM yyyy") : null,
                    StatusManager = g.StatusManager,
                    RemarkManager = g.RemarkManager,
                    approvalDateManager = g.ApprovalDateManager.HasValue ? g.ApprovalDateManager.Value.ToString("dd MMM yyyy") : null,
                    StatusESH = g.StatusESH,
                    RemarkESH = g.RemarkESH,
                    approvalDateESH = g.ApprovalDateESH.HasValue ? g.ApprovalDateESH.Value.ToString("dd MMM yyyy") : null,
                }).ToList();

            return Json(new { rows = employees });
        }


        //APPROVE MANAGER
        [HttpPost]
        public async Task<IActionResult> Approve(int? id)
        {
            var chemical = await _context.Chemicals.FindAsync(id);

            if (chemical == null)
            {
                return NotFound();
            }

            chemical.StatusManager = true;
            _context.Update(chemical);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Chemical approved successfully!!!!" });
        }

        //REJECT MANAGER
        [HttpPost]
        public async Task<IActionResult> Reject(int? id)
        {
            var chemical = await _context.Chemicals.FindAsync(id);

            if (chemical == null)
            {
                return NotFound();
            }

            chemical.StatusManager = false;
            _context.Update(chemical);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Chemical rejected successfully!!!!" });
        }

        private bool Chemical_FALabExists(int id)
        {
            return _context.Chemicals.Any(e => e.Id == id);
        }
    }
}
