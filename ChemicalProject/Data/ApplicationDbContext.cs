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
	    public DbSet<Role> Roles { get; set; }
		public DbSet<User> Users { get; set; }
        public DbSet<Area> Areas { get; set; }
    }
}
