using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplicationApp.Date.Entities;
using WebApplicationApp.Models;

namespace WebApplicationApp.Date
{
    public class SqlDbContext : IdentityDbContext<ApplicationUser>
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
        {
        }

        public DbSet<SalesMan> SalesMans { get; set; }
        public DbSet<SalesMaster> SalesMasters { get; set; }
        public DbSet<SalesDetail> SalesDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // THIS LINE FIXES THE ERROR
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SalesReportDto>().HasNoKey();
        }
    }
}