using ChemicalProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChemicalProject.Helper
{
    public class UserAreaService
    {
        private readonly ApplicationDbContext _context;

        public UserAreaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int?> GetUserAreaIdAsync(ClaimsPrincipal user)
        {
            var windowsUsername = user.Identity.Name;

            if (!string.IsNullOrEmpty(windowsUsername))
            {
                int? areaId = null;

                var userAdmin = await _context.UserAdmins.FirstOrDefaultAsync(u => u.UserName == windowsUsername);
                if (userAdmin != null)
                {
                    areaId = null;
                }

                var userSupervisor = await _context.UserSuperVisors.FirstOrDefaultAsync(u => u.UserName == windowsUsername);
                if (userSupervisor != null)
                {
                    areaId = userSupervisor.AreaId;
                }

                var userManager = await _context.UserManagers.FirstOrDefaultAsync(u => u.UserName == windowsUsername);
                if (userManager != null)
                {
                    areaId = userManager.AreaId;
                }

                var userArea = await _context.UserAreas.FirstOrDefaultAsync(u => u.UserName == windowsUsername);
                if (userArea != null)
                {
                    areaId = userArea.AreaId;
                }

                return areaId;
            }

            return null;
        }
    }
}
