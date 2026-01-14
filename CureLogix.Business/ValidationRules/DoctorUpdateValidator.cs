using CureLogix.Entity.DTOs.DoctorDTOs;
using FluentValidation;

namespace CureLogix.Business.ValidationRules
{
    public class DoctorUpdateValidator : AbstractValidator<DoctorUpdateDto>
    {
        public DoctorUpdateValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Ad Soyad zorunludur!");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Ünvan zorunludur!");
            RuleFor(x => x.Specialty).NotEmpty().WithMessage("Branş zorunludur!");
            RuleFor(x => x.RoleType)
                .NotEmpty().WithMessage("Rol seçimi zorunludur!")
                .GreaterThan(0).WithMessage("Geçersiz rol seçimi! (Lütfen tekrar seçiniz)");
        }
    }
}