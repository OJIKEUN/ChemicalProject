using ChemicalProject.Data;
using ChemicalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ChemicalProject.Helper
{
    public interface IUserService
    {
        Task<User> GetUserByWindowsAccount(string windowsAccount);
        Task<string> GetUserRoleByWindowsAccount(string windowsAccount);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByWindowsAccount(string windowsAccount)
        {
            return await _context.Users.Include(u => u.Role)
                                       .Include(u => u.Area)
                                       .SingleOrDefaultAsync(u => u.UserName == windowsAccount);
        }

        public async Task<string> GetUserRoleByWindowsAccount(string windowsAccount)
        {
            var user = await GetUserByWindowsAccount(windowsAccount);
            return user?.Role.Name;
        }
    }
}
