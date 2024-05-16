using Microsoft.EntityFrameworkCore;
using ChemicalProject.Models;

namespace ChemicalProject.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Chemical_FALab> Chemicals { get; set; }
		public DbSet<Records_FALab> Records { get; set; }
	    public DbSet<Waste_FALab> Wastes { get; set; }
        public DbSet<Area> Areas { get; set; }
		public DbSet<UserAdmin> UserAdmins { get; set; }
        public DbSet<UserManager> UserManagers { get; set; }
        public DbSet<UserSupervisor> UserSupervisors { get; set; }
        public DbSet<UserArea> UserAreas { get; set; }
    }
}
