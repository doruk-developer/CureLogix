using AutoMapper;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.DiseaseDTOs;
using CureLogix.Entity.DTOs.HospitalDTOs;

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
        }
    }
}