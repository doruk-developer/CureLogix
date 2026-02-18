using CureLogix.Entity.Concrete;
using CureLogix.Entity.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CureLogix.DataAccess.Concrete
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(CureLogixContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            // 1. Veritabanı yoksa oluştur
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
			// 3. Varsayılan Kullanıcılar (Identity)
			// ------------------------------------------------------------
			// A. ADMIN HESABI KONTROLÜ
			var adminCheck = await userManager.FindByNameAsync("Admin");
			if (adminCheck == null)
			{
				// Şifre Belirleme (Canlıda Gizli, Yerelde Standart)
				string? liveAdminSecret = Environment.GetEnvironmentVariable("LIVE_ADMIN_PASSWORD");
				string adminPass = !string.IsNullOrEmpty(liveAdminSecret) ? liveAdminSecret : "Admin123!";

				var admin = new AppUser
				{
					UserName = "Admin",
					Email = "admin@curelogix.com",
					EmailConfirmed = true,
					// 👇 BURALAR EKSİKTİ, ŞİMDİ EKLENDİ (DB Hatasını Çözer)
					NameSurname = "Sistem Yöneticisi",
					Title = "Başhekim / Sistem Mimarı"
				};

				var adminResult = await userManager.CreateAsync(admin, adminPass);
				if (adminResult.Succeeded)
				{
					await userManager.AddToRoleAsync(admin, "Admin");
				}
			}

			// B. USER HESABI KONTROLÜ (DEMO / VİTRİN ERİŞİMİ)
			var userCheck = await userManager.FindByNameAsync("User");
			if (userCheck == null)
			{
				var demoUser = new AppUser
				{
					UserName = "User",
					Email = "user@curelogix.com",
					EmailConfirmed = true,
					// 👇 BURALAR EKSİKTİ, ŞİMDİ EKLENDİ
					NameSurname = "İnceleme Kullanıcısı",
					Title = "Sistem Ziyaretçisi"
				};

				// Şifresi senin istediğin gibi sabit: CureLogix123!
				var userResult = await userManager.CreateAsync(demoUser, "CureLogix123!");
				if (userResult.Succeeded)
				{
					await userManager.AddToRoleAsync(demoUser, "User");
				}
			}

			// ------------------------------------------------------------
			// 4. HASTANELER
			// ------------------------------------------------------------
			if (!context.Hospitals.Any())
            {
                context.Hospitals.AddRange(new List<Hospital>
                {
                    new Hospital { Name = "Metropol Merkez Eğitim ve Araştırma", City = "İstanbul", MainStorageCapacity = 50000, OccupancyRate = 92, IsActive = true, WasteStorageCapacity = 5000 },
                    new Hospital { Name = "Anadolu Şehir Hastanesi", City = "Ankara", MainStorageCapacity = 45000, OccupancyRate = 65, IsActive = true, WasteStorageCapacity = 4000 },
                    new Hospital { Name = "Körfez Tıp Fakültesi Hastanesi", City = "İzmir", MainStorageCapacity = 30000, OccupancyRate = 45, IsActive = true, WasteStorageCapacity = 3000 },
                    new Hospital { Name = "Güney Bölge Onkoloji Merkezi", City = "Antalya", MainStorageCapacity = 25000, OccupancyRate = 78, IsActive = true, WasteStorageCapacity = 2000 },
                    new Hospital { Name = "Kuzey Yıldızı Tıp Merkezi", City = "Trabzon", MainStorageCapacity = 20000, OccupancyRate = 30, IsActive = true, WasteStorageCapacity = 1500 },
                    new Hospital { Name = "Doğu İleri Teknoloji Hastanesi", City = "Erzurum", MainStorageCapacity = 15000, OccupancyRate = 55, IsActive = true, WasteStorageCapacity = 1000 }
                });
                context.SaveChanges();
            }

            // ------------------------------------------------------------
            // 5. İLAÇLAR
            // ------------------------------------------------------------
            if (!context.Medicines.Any())
            {
                context.Medicines.AddRange(new List<Medicine>
                {
                    new Medicine { Name = "ViruGuard 500mg", ActiveIngredient = "Oseltamivir", Unit = "Kutu", CriticalStockLevel = 100, RequiresColdChain = false, ShelfLifeDays = 730 },
                    new Medicine { Name = "ImmunoZinc (Takviye)", ActiveIngredient = "Zinc/VitC", Unit = "Şişe", CriticalStockLevel = 200, RequiresColdChain = false, ShelfLifeDays = 365 },
                    new Medicine { Name = "CardioFix 10mg", ActiveIngredient = "Ramipril", Unit = "Kutu", CriticalStockLevel = 50, RequiresColdChain = false, ShelfLifeDays = 1095 },
                    new Medicine { Name = "Adrenalin 1mg Ampul", ActiveIngredient = "Epinephrine", Unit = "Ampul", CriticalStockLevel = 20, RequiresColdChain = true, ShelfLifeDays = 540 },
                    new Medicine { Name = "Pfizer-BioNTech (Comirnaty)", ActiveIngredient = "mRNA", Unit = "Flakon", CriticalStockLevel = 500, RequiresColdChain = true, ShelfLifeDays = 180 },
                    new Medicine { Name = "Parol 500mg", ActiveIngredient = "Parasetamol", Unit = "Kutu", CriticalStockLevel = 1000, RequiresColdChain = false, ShelfLifeDays = 1095 }
                });
                context.SaveChanges();
            }

            // ------------------------------------------------------------
            // 6. DOKTORLAR (Talepler için şart!)
            // ------------------------------------------------------------
            if (!context.Doctors.Any())
            {
                var hospitalId = context.Hospitals.First().Id;
                context.Doctors.AddRange(new List<Doctor>
                {
                    new Doctor { FullName = "Prof. Dr. Kemal Sayar", Title = "Başhekim", Specialty = "Enfeksiyon Hastalıkları", HospitalId = hospitalId, RoleType = 1, Email = "kemal.sayar@curelogix.com" },
                    new Doctor { FullName = "Uzm. Dr. Ali Vefa", Title = "Uzman", Specialty = "Genel Cerrahi", HospitalId = hospitalId, RoleType = 0, Email = "ali.vefa@curelogix.com" }
                });
                context.SaveChanges();
            }

            // ------------------------------------------------------------
            // 7. MERKEZ DEPO STOKLARI
            // ------------------------------------------------------------
            if (!context.CentralWarehouses.Any())
            {
                var meds = context.Medicines.ToList();
                foreach (var med in meds)
                {
                    context.CentralWarehouses.Add(new CentralWarehouse
                    {
                        MedicineId = med.Id,
                        Quantity = 5000,
                        BatchNo = "BATCH-" + new Random().Next(1000, 9999),
                        ManufacturingDate = DateTime.Now.AddMonths(-6),
                        ExpiryDate = DateTime.Now.AddYears(2)
                    });
                }
                context.SaveChanges();
            }

            // ------------------------------------------------------------
            // 8. GEÇMİŞ TALEP VERİLERİ (Hatasız Bağlantı)
            // ------------------------------------------------------------
            if (!context.SupplyRequests.Any())
            {
                var rnd = new Random();
                var hospitalIds = context.Hospitals.Select(x => x.Id).ToList();
                var medicineIds = context.Medicines.Select(x => x.Id).ToList();
                var doctorId = context.Doctors.First().Id; // Gerçek bir doktor alıyoruz

                for (int i = 0; i < 50; i++)
                {
                    context.SupplyRequests.Add(new SupplyRequest
                    {
                        HospitalId = hospitalIds[rnd.Next(hospitalIds.Count)],
                        MedicineId = medicineIds[rnd.Next(medicineIds.Count)],
                        RequestingDoctorId = doctorId, // Sabit gerçek doktor
                        RequestQuantity = rnd.Next(50, 500),
                        RequestDate = DateTime.Now.AddDays(-rnd.Next(1, 90)),
                        Status = (int)(RequestStatus)rnd.Next(0, 3)
                    });
                }
                context.SaveChanges();
            }
        }
    }
}