using ChemicalProject.Data;
using ChemicalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ChemicalProject.Helper
{
    public interface IUserService
    {
        Task<User> GetUserByWindowsAccount(string windowsAccount);
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
            return await _context.Users
                .Include(u => u.Area)
                .SingleOrDefaultAsync(u => u.UserName == windowsAccount);
        }
    }


}
