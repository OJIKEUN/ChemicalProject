using ChemicalProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;

namespace ChemicalProject.Helper
{
    public class UserAreaService
    {
        private readonly ApplicationDbContext _context;
        public UserAreaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetUserAreaIdsAsync(ClaimsPrincipal user)
        {
            var windowsUsername = user.Identity.Name;
            if (!string.IsNullOrEmpty(windowsUsername))
            {
                var areaIds = new List<int>();

                var userAdmin = await _context.UserAdmins.FirstOrDefaultAsync(u => u.UserName == windowsUsername);
                if (userAdmin != null)
                {
                    // Admin memiliki akses ke semua area
                    return await _context.Areas.Select(a => a.Id).ToListAsync();
                }

                var userSupervisor = await _context.UserSuperVisors.Where(u => u.UserName == windowsUsername).ToListAsync();
                areaIds.AddRange(userSupervisor.Select(u => u.AreaId));

                var userManager = await _context.UserManagers.Where(u => u.UserName == windowsUsername).ToListAsync();
                areaIds.AddRange(userManager.Select(u => u.AreaId));

                var userArea = await _context.UserAreas.Where(u => u.UserName == windowsUsername).ToListAsync();
                areaIds.AddRange(userArea.Select(u => u.AreaId));

                return areaIds.Distinct().ToList();
            }

            return new List<int>();
        }
    }
}