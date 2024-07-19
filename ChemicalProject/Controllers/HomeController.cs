using ChemicalProject.Data;
using ChemicalProject.Helper;
using ChemicalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ChemicalProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserAreaService _userAreaService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserAreaService userAreaService)
        {
            _logger = logger;
            _context = context;
            _userAreaService = userAreaService;
        }
        public IActionResult Index()
        {
            ViewBag.BAT1FALabCount = _context.Chemicals
                .Count(c => c.Area.Name == "BAT1 FA LAB" && c.StatusManager == true && c.StatusESH == true);
            ViewBag.BAT1FacilityCount = _context.Chemicals
                .Count(c => c.Area.Name == "BAT1 Facility" && c.StatusManager == true && c.StatusESH == true);
            ViewBag.BAT1A2PlatingCount = _context.Chemicals
                .Count(c => c.Area.Name == "BAT1 A2 Plating" && c.StatusManager == true && c.StatusESH == true);
            ViewBag.BAT1PlatingCount = _context.Chemicals
                .Count(c => c.Area.Name == "BAT1 Plating" && c.StatusManager == true && c.StatusESH == true);
            ViewBag.BAT3FacilityCount = _context.Chemicals
                .Count(c => c.Area.Name == "BAT3 Facility" && c.StatusManager == true && c.StatusESH == true);
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
