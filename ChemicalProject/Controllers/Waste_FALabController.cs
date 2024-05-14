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

        // GET INDEX
        public IActionResult Index(int chemicalId)
        {
            var chemical = _context.Chemicals.FirstOrDefault(c => c.Id == chemicalId);
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

        //GET DATA
        public IActionResult GetData(int id)
        {
            var records = _context.Records
                .Where(r => r.ChemicalId == id)
                .Include(r => r.Chemical_FALab)
                .Include(r => r.Waste)
                .Select(r => new
                {
                    id = r.Id,
                    idRecord = r.Id,
                    chemicalName = r.Chemical_FALab.ChemicalName,
                    consumption = r.Consumption,
                    badge = r.Waste != null ? r.Waste.Badge : 0,
                    wasteType = r.Waste != null ? r.Waste.WasteType : null,
                    wasteQuantity = r.Waste != null ? r.Waste.WasteQuantity : 0,
                    wasteDate = r.Waste != null ? r.Waste.WasteDate.HasValue ? r.Waste.WasteDate.Value.ToString("dd MMM yyyy HH:mm") : null : null,
                    balance = (r.Waste == null) ? (int?)null : (r.Consumption - r.Waste.WasteQuantity)  // Modifikasi di baris ini
                })
                .ToList();

            return Json(new { rows = records });
        }


        //ADD WASTE
        [HttpPost]
        public IActionResult AddWaste(int idRecord, string wasteType, int wasteQuantity, DateTime wasteDate, int badge)
        {
            var record = _context.Records.FirstOrDefault(r => r.Id == idRecord);
            if (record == null)
            {
                return NotFound();
            }

            if (record.Waste != null)
            {
                record.Waste.WasteType = wasteType;
                record.Waste.WasteQuantity = wasteQuantity;
                record.Waste.WasteDate = wasteDate;
                record.Waste.Badge = badge; 
            }
            else
            {
                var waste = new Waste_FALab
                {
                    WasteType = wasteType,
                    WasteQuantity = wasteQuantity,
                    WasteDate = wasteDate,
                    Badge = badge 
                };
                record.Waste = waste;
                _context.Wastes.Add(waste);
            }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            _context.SaveChanges();
            return Json(new { success = true, message = "Waste added/updated successfully." });
        }
    }
}
