using CureLogix.Entity.DTOs.DoctorDTOs;
using FluentValidation;

namespace CureLogix.Business.ValidationRules
{
    public class DoctorValidator : AbstractValidator<DoctorAddDto>
    {
        public DoctorValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Ad Soyad zorunludur!");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Ünvan zorunludur!");
            RuleFor(x => x.Specialty).NotEmpty().WithMessage("Branş zorunludur!");
            RuleFor(x => x.RoleType).NotEmpty().WithMessage("Rol seçimi yapılmalıdır!");
            // HospitalId zorunlu değil, çünkü Konsey üyeleri bağımsız olabilir.
        }
    }
}