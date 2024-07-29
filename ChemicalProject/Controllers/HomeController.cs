using ChemicalProject.Data;
using ChemicalProject.Helper;
using ChemicalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        public async Task<IActionResult> Index()
        {
            var userAreaIds = await _userAreaService.GetUserAreaIdsAsync(User);
            var isAdmin = User.IsInRole("UserAdmin") || User.IsInRole("UserManager");

            var areaChemicalCounts = await _context.Areas
                .Select(area => new
                {
                    AreaId = area.Id,
                    AreaName = area.Name,
                    ChemicalCount = _context.Chemicals.Count(c => c.AreaId == area.Id && c.StatusManager == true && c.StatusESH == true),
                    HasAccess = isAdmin || userAreaIds.Contains(area.Id)
                })
                .ToListAsync();

            ViewBag.UserAreaIds = userAreaIds;
            ViewBag.IsAdmin = isAdmin;

            return View(areaChemicalCounts);
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
