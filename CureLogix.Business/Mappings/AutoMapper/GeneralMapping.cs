using AutoMapper;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.CouncilDTOs;
using CureLogix.Entity.DTOs.DiseaseDTOs;
using CureLogix.Entity.DTOs.DoctorDTOs;
using CureLogix.Entity.DTOs.HospitalDTOs;
using CureLogix.Entity.DTOs.MedicineDTOs;
using CureLogix.Entity.DTOs.ProtocolDTOs;
using CureLogix.Entity.DTOs.SupplyDTOs;
using CureLogix.Entity.DTOs.UserDTOs;
using CureLogix.Entity.DTOs.WarehouseDTOs;
using CureLogix.Entity.DTOs.WasteDTOs;

namespace CureLogix.Business.Mappings.AutoMapper
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            // Hospital tablosunu -> HospitalListDto'ya dönüştür (ve tersi)
            CreateMap<Hospital, HospitalListDto>().ReverseMap();
            // İleride diğerlerini de buraya ekleyeceğiz (Doctor, Medicine vb.)

            // Ekleme İşlemi İçin: DTO -> Entity
            CreateMap<HospitalAddDto, Hospital>();

            // Hastalıklar
            CreateMap<Disease, DiseaseListDto>().ReverseMap();
            CreateMap<DiseaseAddDto, Disease>().ReverseMap();

            // Protokol Eşleştirmeleri
            CreateMap<ProtocolCreateDto, TreatmentProtocol>()
                .ForMember(dest => dest.ProtocolMedicines, opt => opt.MapFrom(src => src.Medicines));

            // Yukarıdaki satır: DTO'daki 'Medicines' listesini, Entity'deki 'ProtocolMedicines' listesine dök.
            CreateMap<ProtocolMedicineDto, ProtocolMedicine>();

            // İlaçlar
            CreateMap<Medicine, MedicineListDto>().ReverseMap();

            // İlaç Ekleme
            CreateMap<MedicineAddDto, Medicine>();

            // 1. Protokol Detay Dönüşümü (Zor kısım burası)
            CreateMap<TreatmentProtocol, ProtocolDetailDto>()
                .ForMember(dest => dest.DiseaseName, opt => opt.MapFrom(src => src.Disease.Name))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FullName))
                .ForMember(dest => dest.Medicines, opt => opt.MapFrom(src => src.ProtocolMedicines));

            // İlaç detay dönüşümü
            CreateMap<ProtocolMedicine, ProtocolMedicineViewDto>()
                .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Medicine.Name))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.RequiredQuantity))
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.DosageInstructions));

            // 2. Oy Verme Dönüşümü
            CreateMap<VoteOperationDto, CouncilVote>()
                .ForMember(dest => dest.Vote, opt => opt.MapFrom(src => src.VoteResult));

            // Doktorlar-Ünvan/Role
            CreateMap<Doctor, DoctorListDto>()
                .ForMember(dest => dest.HospitalName, opt => opt.MapFrom(src => src.Hospital.Name));

            CreateMap<DoctorAddDto, Doctor>();

            // Merkez Depo
            CreateMap<CentralWarehouse, CentralStockListDto>()
                .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Medicine.Name))
                .ForMember(dest => dest.MedicineUnit, opt => opt.MapFrom(src => src.Medicine.Unit));

            // Merkez Depo Yönetimi için
            CreateMap<CentralStockAddDto, CentralWarehouse>();

            // Talep Yönetimi için
            CreateMap<SupplyRequestAddDto, SupplyRequest>();

            // Atık Yönetimi için
            CreateMap<WasteReport, WasteReportListDto>().ForMember(x => x.HospitalName, o => o.MapFrom(s => s.Hospital.Name)).ForMember(x => x.MedicineName, o => o.MapFrom(s => s.Medicine.Name));

            // Hospital/Update Mappingi
            CreateMap<HospitalUpdateDto, Hospital>().ReverseMap();

            // Doktor Güncelleme Mappingi
            CreateMap<DoctorUpdateDto, Doctor>().ReverseMap();

            // Hastalık Güncelleme Mappingi
            CreateMap<DiseaseUpdateDto, Disease>().ReverseMap();

            // İlaç Güncelleme Mappingi
            CreateMap<MedicineUpdateDto, Medicine>().ReverseMap();

            // Yeni Kullanıcı Ekleme Mappingi
            CreateMap<UserAddDto, AppUser>();

            // Kullanıcı Güncelleme Mappingi
            CreateMap<UserUpdateDto, AppUser>().ReverseMap();
        }
    }
}