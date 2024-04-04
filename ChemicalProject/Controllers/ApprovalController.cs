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
    public class ApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApprovalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Chemical_FALab
        public IActionResult Index()
        {
            // Ambil data bahan kimia dengan status null
            var chemicalsToApprove = _context.Chemicals.Where(c => c.Status == null).ToList();
            return View(chemicalsToApprove);
        }

        public JsonResult GetData()
        {
            var chemicalsToApprove = _context.Chemicals.Where(c => c.Status == null).ToList();
            return Json(chemicalsToApprove);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var chemical = await _context.Chemicals.FindAsync(id);
            if (chemical == null)
            {
                return NotFound();
            }

            chemical.Status = true; // Ubah status menjadi true untuk approval
            _context.Update(chemical);
            await _context.SaveChangesAsync();

            // Redirect ke halaman index setelah menyetujui
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var chemical = await _context.Chemicals.FindAsync(id);
            if (chemical == null)
            {
                return NotFound();
            }

            chemical.Status = false; // Ubah status menjadi false untuk rejection
            _context.Update(chemical);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
