using HLSMP.Models;
using Microsoft.EntityFrameworkCore;

namespace HLSMP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<LoginDetail> LoginDetails { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginDetail>().ToTable("LoginDetails");
        }
    }
}
