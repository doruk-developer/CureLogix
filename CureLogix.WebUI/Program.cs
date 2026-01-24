using CureLogix.Business.Abstract;
using CureLogix.Business.Concrete;
using CureLogix.Business.ValidationRules;
using CureLogix.DataAccess.Abstract;
using CureLogix.DataAccess.Concrete;
using CureLogix.DataAccess.Repositories;
using CureLogix.Entity.Concrete;
using CureLogix.WebUI.Hubs;
using CureLogix.WebUI.Middlewares;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ==================================================================
// 1. AKILLI VERITABANI BAGLANTISI (SMART CONNECTION)
// ==================================================================

var machineName = Environment.MachineName;
string connectionStringName;

if (machineName == "N56VZ-DORUK")
{
    connectionStringName = "HomeConnection";
}
else
{
    connectionStringName = "WorkConnection"; // Varsayılan İş Bilgisayarı
}

var connectionString = builder.Configuration.GetConnectionString(connectionStringName)
                       ?? throw new InvalidOperationException($"HATA: '{connectionStringName}' bulunamadı!");

// DbContext Kurulumu
builder.Services.AddDbContext<CureLogixContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("CureLogix.DataAccess")));

// ==================================================================
// 2. IDENTITY (KİMLİK) VE GÜVENLİK YAPILANDIRMASI
// ==================================================================

builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<CureLogixContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Şifre Politikası
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;

    // Kilitleme (Brute Force Koruması)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    options.Lockout.MaxFailedAccessAttempts = 5;
});

// Çerez Ayarları
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Index";
    options.AccessDeniedPath = "/Login/Index";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

// GLOBAL AUTHORIZATION ve JSON AYARLARI
builder.Services.AddControllersWithViews(config =>
{
    // 1. Global Kilit (Giriş yapmayan hiçbir sayfayı göremez)
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();

    config.Filters.Add(new AuthorizeFilter(policy));
})
.AddJsonOptions(options =>
{
    // 2. Sonsuz Döngü (Circular Reference) Engelleme - API için kritik!
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

    // 3. JSON Format Ayarı (İsimleri olduğu gibi PascalCase bırakır)
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// ==================================================================
// 3. DEPENDENCY INJECTION (SERVİS KAYITLARI)
// ==================================================================

// Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Business Services (Managerlar)
builder.Services.AddScoped<IHospitalService, HospitalManager>();
builder.Services.AddScoped<IDoctorService, DoctorManager>();
builder.Services.AddScoped<IMedicineService, MedicineManager>();
builder.Services.AddScoped<ITreatmentProtocolService, TreatmentProtocolManager>();
builder.Services.AddScoped<IDiseaseService, DiseaseManager>();
builder.Services.AddScoped<ICouncilVoteService, CouncilVoteManager>();
builder.Services.AddScoped<ICentralWarehouseService, CentralWarehouseManager>();
builder.Services.AddScoped<ISupplyRequestService, SupplyRequestManager>();
builder.Services.AddScoped<IVehicleService, VehicleManager>();
builder.Services.AddScoped<IQrCodeService, QrCodeManager>();
builder.Services.AddScoped<IWasteReportService, WasteReportManager>();
builder.Services.AddScoped<IAuditLogService, AuditLogManager>();

// AI Servisi (Singleton)
builder.Services.AddSingleton<IAiForecastService, AiForecastManager>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(CureLogix.Business.Mappings.AutoMapper.GeneralMapping));

// SignalR
builder.Services.AddSignalR();

// ==================================================================
// 4. HANGFIRE KURULUMU (ARKA PLAN İŞLERİ)
// ==================================================================

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString));

builder.Services.AddHangfireServer();

// 5. SWAGGER KONFİGÜRASYONU (API Dökümantasyonu)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CureLogix Enterprise API",
        Version = "v1",
        Description = "Sağlık Lojistiği ve Stok Yönetimi Entegrasyon Servisi",
        Contact = new OpenApiContact { Name = "CureLogix Dev Team" }
    });
});


// 6. UYGULAMA PIPELINE (MIDDLEWARE)
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CureLogix API v1");
    c.RoutePrefix = "api-docs";
});

// HATA YÖNETİMİ
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error/Page500");
//    app.UseHsts();
//}
//else
//{
//    app.UseExceptionHandler("/Error/Page500");
//}

app.UseDeveloperExceptionPage();

app.UseStatusCodePagesWithReExecute("/Error/Page404");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ÖZEL IP KISITLAMA MODÜLÜ
app.UseMiddleware<IpSafeListMiddleware>();

// ✅ YENİ: VERİTABANI KORUMA KALKANI (AUTH'TAN ÖNCE OLMALI)
app.UseMiddleware<DbFailSafeMiddleware>();

// Güvenlik Sıralaması
app.UseAuthentication();
app.UseAuthorization();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

// ==================================================================
// 6. GÜVENLİ BAŞLANGIÇ GÖREVLERİ (FAIL-SAFE ZONE)
// ==================================================================
// Buradaki işlemler veritabanına bağlıdır. Bağlantı yoksa patlamamalı,
// log yazıp uygulamayı açmaya devam etmelidir.

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // A) HANGFIRE GÖREVLERİ
    try
    {
        var recurringJobManager = services.GetRequiredService<IRecurringJobManager>();
        var warehouseService = services.GetRequiredService<ICentralWarehouseService>();

        recurringJobManager.AddOrUpdate(
            "Otomatik-Siparis-Olusturma",
            () => warehouseService.CheckExpiriesAndCreateOrder(),
            Cron.Daily(3) // Her gece 03:00
        );
    }
    catch (Exception ex)
    {
        // DB yoksa Hangfire görevi kurulamaz, ama site açılsın.
        Console.WriteLine($"⚠️ UYARI: Hangfire Job başlatılamadı (DB Hatası): {ex.Message}");
    }

    // B) VERİ TOHUMLAMA (SEED DATA)
    try
    {
        var context = services.GetRequiredService<CureLogixContext>();

        // Veritabanı ile el sıkış (Ping at)
        if (context.Database.CanConnect())
        {
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

            // Veritabanı yoksa oluştur (Migrationları bas)
            // context.Database.EnsureCreated(); // Bu satır bazen Migration çakışması yaratabilir, dikkatli kullanılmalı.
            // Eğer Migration kullanıyorsan: context.Database.Migrate();

            // Verileri Doldur
            await DbSeeder.SeedAsync(context, userManager, roleManager);
        }
    }
    catch (Exception ex)
    {
        // DB yoksa Seed yapılamaz, ama site açılsın.
        Console.WriteLine($"⚠️ UYARI: Seed Data atlandı (DB Hatası): {ex.Message}");
    }
}

// Endpoint Tanımları
app.MapHub<GeneralHub>("/generalHub"); // SignalR

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();