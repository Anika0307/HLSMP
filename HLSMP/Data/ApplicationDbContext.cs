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
        public DbSet<TatimaSummary> TatimaSummarys { get; set; }
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

            modelBuilder.Entity<TatimaSummary>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null);  // Optional: prevents EF from mapping this to a DB view/table
            });

            base.OnModelCreating(modelBuilder);


            //modelBuilder.Entity<VillageTatima>().HasData(
            //    new VillageTatima
            //    {
            //        VillageCode = 00001, // Primary Key
            //        Dist_Code = 01,
            //        Teh_Code = 001,
            //        TotalTatima = 50,
            //        Completed = 30,
            //        Pending = 20,
            //        StatusCode = 2
            //    },
            //    new VillageTatima
            //    {
            //        VillageCode = 00032, 
            //        Dist_Code = 01,
            //        Teh_Code = 006,
            //        TotalTatima = 60,
            //        Completed = 30,
            //        Pending = 30,
            //        StatusCode = 3
            //    },
            //    new VillageTatima
            //    {
            //        VillageCode = 01931,
            //        Dist_Code = 02,
            //        Teh_Code = 007,
            //        TotalTatima = 70,
            //        Completed = 30,
            //        Pending = 40,
            //        StatusCode = 4
            //    },
            //    new VillageTatima
            //    {
            //        VillageCode = 01982, 
            //        Dist_Code = 02,
            //        Teh_Code = 011,
            //        TotalTatima = 70,
            //        Completed = 30,
            //        Pending = 40,
            //        StatusCode = 5
            //    },
            //    new VillageTatima
            //    {
            //        VillageCode = 02110,
            //        Dist_Code = 02,
            //        Teh_Code = 010,
            //        TotalTatima = 60,
            //        Completed = 60,
            //        Pending = 0,
            //        StatusCode = 7
            //    }
            //);

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
