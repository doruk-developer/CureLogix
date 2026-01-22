using CureLogix.Entity.Concrete;
using CureLogix.Entity.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CureLogix.DataAccess.Concrete
{
    public static class DbSeeder
    {
        // Metot ve Sınıf PUBLIC olmalı
        public static async Task SeedAsync(CureLogixContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            // 1. Veritabanı yoksa oluştur
            // (Migration kullanıyorsan burası sadece veri basmalı, tablo oluşturmamalı. 
            // Ancak sıfırdan kurulumda hayat kurtarır)
            context.Database.EnsureCreated();

            // ------------------------------------------------------------
            // 2. ROLLER (Admin ve User)
            // ------------------------------------------------------------
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new AppRole { Name = "Admin" });
                await roleManager.CreateAsync(new AppRole { Name = "User" });
            }

            // ------------------------------------------------------------
            // 3. VARSAYILAN YÖNETİCİ (Login için)
            // ------------------------------------------------------------
            if (!userManager.Users.Any())
            {
                var admin = new AppUser
                {
                    UserName = "Admin",
                    Email = "admin@curelogix.com",
                    NameSurname = "Sistem Yöneticisi",
                    Title = "Başhekim / Sistem Mimarı",
                    EmailConfirmed = true,
                    ProfilePicture = "default-admin.jpg" // Varsayılan bir resim atadık
                };

                // Şifre: CureLogix123!
                await userManager.CreateAsync(admin, "CureLogix123!");
                await userManager.AddToRoleAsync(admin, "Admin");

                // Demo için bir de standart doktor ekleyelim
                var doctor = new AppUser
                {
                    UserName = "Doktor",
                    Email = "doktor@curelogix.com",
                    NameSurname = "Dr. Ali Vefa",
                    Title = "Uzman Doktor",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(doctor, "Doktor123!");
                await userManager.AddToRoleAsync(doctor, "User");
            }

            // ------------------------------------------------------------
            // 4. HASTANELER (Harita ve Doluluk Grafiği için)
            // ------------------------------------------------------------
            if (!context.Hospitals.Any())
            {
                context.Hospitals.AddRange(new List<Hospital>
                {
                    new Hospital { Name = "Metropol Merkez Eğitim ve Araştırma", City = "İstanbul", MainStorageCapacity = 50000, OccupancyRate = 92, IsActive = true },
                    new Hospital { Name = "Anadolu Şehir Hastanesi", City = "Ankara", MainStorageCapacity = 45000, OccupancyRate = 65, IsActive = true },
                    new Hospital { Name = "Körfez Tıp Fakültesi", City = "İzmir", MainStorageCapacity = 30000, OccupancyRate = 45, IsActive = true },
                    new Hospital { Name = "Güney Bölge Onkoloji Merkezi", City = "Antalya", MainStorageCapacity = 25000, OccupancyRate = 78, IsActive = true },
                    new Hospital { Name = "Kuzey Yıldızı Tıp Merkezi", City = "Trabzon", MainStorageCapacity = 20000, OccupancyRate = 30, IsActive = true },
                    new Hospital { Name = "Doğu İleri Teknoloji Hastanesi", City = "Erzurum", MainStorageCapacity = 15000, OccupancyRate = 55, IsActive = true }
                });
                context.SaveChanges();
            }

            // ------------------------------------------------------------
            // 5. İLAÇLAR (Stok Takibi ve Radar Grafiği için)
            // ------------------------------------------------------------
            if (!context.Medicines.Any())
            {
                context.Medicines.AddRange(new List<Medicine>
                {
                    new Medicine { Name = "ViruGuard 500mg", ActiveIngredient = "Oseltamivir", Unit = "Kutu", CriticalStockLevel = 100, RequiresColdChain = false },
                    new Medicine { Name = "ImmunoZinc (Takviye)", ActiveIngredient = "Zinc/VitC", Unit = "Şişe", CriticalStockLevel = 200, RequiresColdChain = false },
                    new Medicine { Name = "CardioFix 10mg", ActiveIngredient = "Ramipril", Unit = "Kutu", CriticalStockLevel = 50, RequiresColdChain = false },
                    new Medicine { Name = "Adrenalin 1mg Ampul", ActiveIngredient = "Epinephrine", Unit = "Ampul", CriticalStockLevel = 20, RequiresColdChain = true },
                    new Medicine { Name = "Pfizer-BioNTech (Comirnaty)", ActiveIngredient = "mRNA", Unit = "Flakon", CriticalStockLevel = 500, RequiresColdChain = true },
                    new Medicine { Name = "Parol 500mg", ActiveIngredient = "Parasetamol", Unit = "Kutu", CriticalStockLevel = 1000, RequiresColdChain = false }
                });
                context.SaveChanges();
            }

            // ------------------------------------------------------------
            // 6. MERKEZ DEPO STOKLARI (Kritik Stok Uyarısı Almamak İçin)
            // ------------------------------------------------------------
            if (!context.CentralWarehouses.Any())
            {
                var meds = context.Medicines.ToList();
                foreach (var med in meds)
                {
                    context.CentralWarehouses.Add(new CentralWarehouse
                    {
                        MedicineId = med.Id,
                        Quantity = 5000, // Her ilaçtan 5000 tane var
                        BatchNo = "BATCH-" + new Random().Next(1000, 9999),
                        ManufacturingDate = DateTime.Now.AddMonths(-6),
                        ExpiryDate = DateTime.Now.AddYears(2)
                    });
                }
                context.SaveChanges();
            }

            // ------------------------------------------------------------
            // 7. GEÇMİŞ TALEP VERİLERİ (Grafikleri Canlı Göstermek İçin)
            // ------------------------------------------------------------
            if (!context.SupplyRequests.Any())
            {
                var rnd = new Random();
                var hospitalIds = context.Hospitals.Select(x => x.Id).ToList();
                var medicineIds = context.Medicines.Select(x => x.Id).ToList();

                for (int i = 0; i < 50; i++) // 50 tane rastgele talep oluştur
                {
                    context.SupplyRequests.Add(new SupplyRequest
                    {
                        HospitalId = hospitalIds[rnd.Next(hospitalIds.Count)],
                        MedicineId = medicineIds[rnd.Next(medicineIds.Count)],
                        RequestQuantity = rnd.Next(50, 500),
                        // Son 3 aya yayılmış tarihler
                        RequestDate = DateTime.Now.AddDays(-rnd.Next(1, 90)),
                        // Rastgele Durum (0: Bekliyor, 1: Onay, 2: Red)
                        Status = (int)(RequestStatus)rnd.Next(0, 3)
                    });
                }
                context.SaveChanges();
            }
        }
    }
}