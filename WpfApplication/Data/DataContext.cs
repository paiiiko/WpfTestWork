using WpfApplication.Interfaces;
using Microsoft.EntityFrameworkCore;
using WpfApplication.Tools;
using WpfApplication.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WpfApplication.Data
{
    public class DataContext : DbContext, ICompaniesDbContext,
                                          IEmloyeesDbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = PathToDir.CalculatePath("Data.db");
            optionsBuilder.UseSqlite($"Data Source={path}");
        }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //}
    }
}