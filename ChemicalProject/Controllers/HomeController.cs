using ChemicalProject.Data;
using ChemicalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ChemicalProject.Controllers
{
	public class HomeController : Controller
	{
        private readonly ILogger<HomeController> logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            this.logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.FALabCount = _context.Chemicals
                .Count(c => c.Area.Name == "FA Lab" && c.StatusManager == true && c.StatusESH == true);

            ViewBag.PlatingCount = _context.Chemicals
                .Count(c => c.Area.Name == "Plating" && c.StatusManager == true && c.StatusESH == true);

            ViewBag.A2PlatingCount = _context.Chemicals
                .Count(c => c.Area.Name == "A2Plating" && c.StatusManager == true && c.StatusESH == true);
            ViewBag.FacilityCount = _context.Chemicals
                .Count(c => c.Area.Name == "Facility" && c.StatusManager == true && c.StatusESH == true);
            ViewBag.BAT3LAB = _context.Chemicals
               .Count(c => c.Area.Name == "BAT3LAB" && c.StatusManager == true && c.StatusESH == true);



            return View();
        }

        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
