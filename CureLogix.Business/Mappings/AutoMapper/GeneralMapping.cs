using AutoMapper;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.DiseaseDTOs;
using CureLogix.Entity.DTOs.HospitalDTOs;
using CureLogix.Entity.DTOs.MedicineDTOs;
using CureLogix.Entity.DTOs.ProtocolDTOs;

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
        }
    }
}