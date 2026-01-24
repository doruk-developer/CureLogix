using CureLogix.Entity.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Identity kütüphanesi
using Microsoft.EntityFrameworkCore;

namespace CureLogix.DataAccess.Concrete
{
    // DİKKAT: Artık DbContext'ten değil, IdentityDbContext'ten miras alıyoruz.
    // <AppUser, AppRole, int>: Kullanıcı sınıfımız, Rol sınıfımız ve ID tipimiz (int).
    public partial class CureLogixContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public CureLogixContext()
        {
        }

        public CureLogixContext(DbContextOptions<CureLogixContext> options)
            : base(options)
        {
        }

        // ==========================================
        // PROJE TABLOLARI (DbSet)
        // ==========================================
        public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Medicine> Medicines { get; set; }
        public virtual DbSet<Disease> Diseases { get; set; }
        public virtual DbSet<TreatmentProtocol> TreatmentProtocols { get; set; }
        public virtual DbSet<ProtocolMedicine> ProtocolMedicines { get; set; }
        public virtual DbSet<CouncilVote> CouncilVotes { get; set; }
        public virtual DbSet<CentralWarehouse> CentralWarehouses { get; set; }
        public virtual DbSet<HospitalInventory> HospitalInventories { get; set; }
        public virtual DbSet<SupplyRequest> SupplyRequests { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<WasteReport> WasteReports { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        // ==========================================
        // KONFİGÜRASYONLAR
        // ==========================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Identity ayarları için şart (EN ÜSTTE OLMALI)
            base.OnModelCreating(modelBuilder);

            // 2. Tablo İsim Sabitlemeleri
            modelBuilder.Entity<CentralWarehouse>().ToTable("CentralWarehouse");
            modelBuilder.Entity<HospitalInventory>().ToTable("HospitalInventory");
            modelBuilder.Entity<WasteReport>().ToTable("WasteReports");
            modelBuilder.Entity<ErrorLog>().ToTable("ErrorLogs");
            modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");

            // 3. ✅ KISIR DÖNGÜYÜ KIRAN KOD (Önceki hatanın çözümü)
            modelBuilder.Entity<CouncilVote>()
                .HasOne(v => v.TreatmentProtocol)
                .WithMany(p => p.CouncilVotes)
                .HasForeignKey(v => v.ProtocolId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}