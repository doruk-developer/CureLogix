using CureLogix.Business.Abstract;
using CureLogix.Business.Concrete;
using CureLogix.Business.ValidationRules; // Validatorlar için gerekebilir
using CureLogix.DataAccess.Abstract;
using CureLogix.DataAccess.Concrete;
using CureLogix.DataAccess.Repositories;
using CureLogix.Entity.Concrete;
using CureLogix.WebUI.Hubs;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

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

// GLOBAL AUTHORIZATION (Tüm Sistemi Kilitleme)
builder.Services.AddControllersWithViews(config =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
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

// ==================================================================
// 5. UYGULAMA PIPELINE (MIDDLEWARE)
// ==================================================================

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); // Routing bir kere tanımlanır

// Güvenlik Sıralaması Önemlidir: Önce Kimlik Doğrulama, Sonra Yetkilendirme
app.UseAuthentication();
app.UseAuthorization();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

// Hangfire Zamanlanmış Görevler (Cron Jobs)
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    var warehouseService = scope.ServiceProvider.GetRequiredService<ICentralWarehouseService>();

    recurringJobManager.AddOrUpdate(
        "Otomatik-Siparis-Olusturma",
        () => warehouseService.CheckExpiriesAndCreateOrder(),
        Cron.Daily(3) // Her gece 03:00
    );
}

// Endpoint Tanımları
app.MapHub<GeneralHub>("/generalHub"); // SignalR

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();