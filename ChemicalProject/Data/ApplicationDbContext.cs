using ChemicalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ChemicalProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        public DbSet<Chemical_FALab> Chemicals { get; set; }
        public DbSet<Records_FALab> Records { get; set; }
        public DbSet<Waste_FALab> Wastes { get; set; }
        public DbSet<UserArea> UserAreas { get; set; }
        public DbSet<UserSuperVisor> UserSuperVisors { get; set; }
        public DbSet<UserManager> UserManagers { get; set; }
        public DbSet<UserAdmin> UserAdmins { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<ActualRecord> ActualRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(
                connectionString,
                x => x.MigrationsHistoryTable("CC_EFMigrationsHistory", "CC_Schema"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("CC_Schema");
            base.OnModelCreating(modelBuilder);
        }
    }
}
