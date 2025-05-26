using HLSMP.Models;
using HLSMP.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace HLSMP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<VillageTatima> VillageTatimas { get; set; }

        public DbSet<LoginDetail> LoginDetails { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }

        public virtual DbSet<DisMa> DisMas { get; set; }

        public virtual DbSet<TblReasonMa> TblReasonMas { get; set; }

        public virtual DbSet<TblRoleMa> TblRoleMas { get; set; }

        public virtual DbSet<TblUser> TblUsers { get; set; }

        public virtual DbSet<TblUserMa> TblUserMas { get; set; }

        public virtual DbSet<TehMasLgdUpdated> TehMasLgdUpdateds { get; set; }

        public virtual DbSet<VilMa> VilMas { get; set; }
        public virtual DbSet<StatusModel> TblStatus { get; set; }





        public IEnumerable<object> Tehsils { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DisMa>(entity =>
            {
                entity.Property(e => e.DisCode).IsFixedLength();
                entity.Property(e => e.DivCode).IsFixedLength();
                entity.Property(e => e.StaCode).IsFixedLength();
                entity.Property(e => e.Version)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<TblUserMa>(entity =>
            {
                entity.HasOne(d => d.Role).WithMany().HasConstraintName("FK_TblUser_MAS_TblRole_MAS");
            });

            modelBuilder.Entity<TehMasLgdUpdated>(entity =>
            {
                entity.Property(e => e.DisCode).IsFixedLength();
                entity.Property(e => e.PtehCode).IsFixedLength();
                entity.Property(e => e.SdoCode).IsFixedLength();
                entity.Property(e => e.TehCode).IsFixedLength();
            });

            modelBuilder.Entity<VilMa>(entity =>
            {
                entity.Property(e => e.Version)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });


        }

    }
}
