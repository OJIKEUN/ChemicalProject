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
    public class Waste_FALabController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Waste_FALabController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int chemicalId)
        {
            var chemical = _context.Chemicals.FirstOrDefault(c => c.Id == chemicalId);
            if (chemical == null)
                if (chemical == null)
                {
                    return NotFound();
                }

            var records = _context.Records
                .Where(r => r.ChemicalId == chemicalId)
                .Include(r => r.Chemical_FALab)
                .ToList();

            var currentStock = records.Sum(r => r.ReceivedQuantity) - records.Sum(r => r.Consumption);

            ViewBag.ChemicalId = chemicalId;
            ViewBag.ChemicalName = chemical.ChemicalName;
            ViewBag.CurrentStock = currentStock;

            return View();
        }

        [HttpGet]
        public IActionResult GetData(int id)
        {
            var wastes = _context.Wastes
                .Where(w => w.Records.ChemicalId == id)
                .Select(w => new
                {
                    idRecord = w.RecordId,
                    chemicalName = w.Records.Chemical_FALab.ChemicalName,
                    consumption = w.Records.Consumption,
                    wasteType = w.WasteType,
                    wasteQuantity = w.WasteQuantity,
                    wasteDate = w.WasteDate.HasValue? w.WasteDate.Value.ToString("dd MMM yyyy HH:mm") : null
                })
                .ToList();

            return Json(new { rows = wastes });
        }

        public IActionResult GetData()
        {
            var Chemicals = _context.Chemicals
                .Select(c => new
                {
                    id = c.Id,
                    badge = c.Badge,
                    chemicalName = c.ChemicalName,
                    brand = c.Brand,
                    packaging = c.Packaging,
                    unit = c.Unit,
                    minimumStock = c.MinimumStock,
                    price = c.Price,
                    justify = c.Justify,
                    requestDate = c.RequestDate.HasValue ? c.RequestDate.Value.ToString("dd MMM yyyy HH:mm") : null,
                    statusManager = c.StatusManager,
                    remarkManager = c.RemarkManager,
                    approvalDateManager = c.ApprovalDateManager.HasValue ? c.ApprovalDateManager.Value.ToString("dd MMM yyyy HH:mm") : null,
                    statusESH = c.StatusESH,
                    remarkESH = c.RemarkESH,
                    approvalDateESH = c.ApprovalDateESH.HasValue ? c.ApprovalDateESH.Value.ToString("dd MMM yyyy HH:mm") : null,
                }).ToList();

            return Json(new { rows = Chemicals });
        }
    }
}
