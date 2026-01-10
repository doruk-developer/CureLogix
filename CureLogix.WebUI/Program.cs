using CureLogix.Business.Abstract;       // IHospitalService vb. için
using CureLogix.Business.Concrete;       // HospitalManager vb. için
using CureLogix.DataAccess.Abstract;     // IGenericRepository için
using CureLogix.DataAccess.Concrete;     // Context için
using CureLogix.DataAccess.Repositories; // GenericRepository için (*)
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==================================================================
// AKILLI VERITABANI BAGLANTISI (MULTI-MACHINE SETUP)
// ==================================================================

// 1. Bilgisayarin adini aliyoruz
var machineName = Environment.MachineName;
string connectionStringName;

// 2. Hangi bilgisayardaysak o ConnectionString ismini seciyoruz
if (machineName == "N56VZ-DORUK")
{
    connectionStringName = "HomeConnection";
}
else
{
    // Is bilgisayari veya diger herhangi bir ortam icin varsayilan
    connectionStringName = "WorkConnection";
}

// 3. appsettings.json dosyasindan baglanti cumlesini cekiyoruz
var connectionString = builder.Configuration.GetConnectionString(connectionStringName)
                       ?? throw new InvalidOperationException($"HATA: '{connectionStringName}' isimli baglanti cumlesi appsettings.json dosyasinda bulunamadi!");

// 4. Context servisini bu baglanti ile ayaga kaldiriyoruz
builder.Services.AddDbContext<CureLogixContext>(options =>
    options.UseSqlServer(connectionString));

// ============================================================
// 2. DEPENDENCY INJECTION (SERVİS BAĞLANTILARI)
// ============================================================

// Repository'ler
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Business Manager'lar
builder.Services.AddScoped<IHospitalService, HospitalManager>();
builder.Services.AddScoped<IDoctorService, DoctorManager>();
builder.Services.AddScoped<IMedicineService, MedicineManager>();
builder.Services.AddScoped<ITreatmentProtocolService, TreatmentProtocolManager>();
builder.Services.AddScoped<IDiseaseService, DiseaseManager>();
// AutoMapper Konfigürasyonu
// Business katmanındaki GeneralMapping sınıfını referans alarak tarama yapar
builder.Services.AddAutoMapper(typeof(CureLogix.Business.Mappings.AutoMapper.GeneralMapping));
builder.Services.AddScoped<ICouncilVoteService, CouncilVoteManager>();

// Central Warehouse Service
builder.Services.AddScoped<ICentralWarehouseService, CentralWarehouseManager>();

// FEFE Algoritması Modülü için
builder.Services.AddScoped<ISupplyRequestService, SupplyRequestManager>();

// Soğuk Zincir Takip Modülü için
builder.Services.AddScoped<IVehicleService, VehicleManager>();

// QRCoder için
builder.Services.AddScoped<IQrCodeService, QrCodeManager>();
// ============================================================

// MVC Servislerini ekle
builder.Services.AddControllersWithViews();

var app = builder.Build();

// HTTP Pipeline (Standart Ayarlar)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();