using CureLogix.Entity.Concrete;
using Microsoft.EntityFrameworkCore;

namespace CureLogix.DataAccess.Concrete
{
    public partial class CureLogixContext : DbContext
    {
        public CureLogixContext()
        {
        }

        // Program.cs tarafından ayarların gönderildiği yer
        public CureLogixContext(DbContextOptions<CureLogixContext> options)
            : base(options)
        {
        }

        // TABLO TANIMLARI (DbSet)
        // Veritabanındaki tabloların C# karşılıkları
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // EF Core'a diyoruz ki: "Senin CentralWarehouses dediğin tablo, SQL'de 'CentralWarehouse' adıyla kayıtlı."
            modelBuilder.Entity<CentralWarehouse>().ToTable("CentralWarehouse");

            base.OnModelCreating(modelBuilder);
        }
    }
}