using CureLogix.Entity.DTOs.HospitalDTOs;
using FluentValidation;

namespace CureLogix.Business.ValidationRules
{
    // Dikkat: <HospitalUpdateDto> için çalışıyor
    public class HospitalUpdateValidator : AbstractValidator<HospitalUpdateDto>
    {
        public HospitalUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Hastane adı boş olamaz!");
            RuleFor(x => x.City).NotEmpty().WithMessage("Şehir boş olamaz!");
            RuleFor(x => x.MainStorageCapacity).GreaterThan(0).WithMessage("Kapasite pozitif olmalı!");
        }
    }
}