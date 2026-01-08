using CureLogix.Entity.DTOs.HospitalDTOs;
using FluentValidation;

namespace CureLogix.Business.ValidationRules
{
    public class HospitalValidator : AbstractValidator<HospitalAddDto>
    {
        public HospitalValidator()
        {
            // Hastane Adı Kuralları
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Hastane adı boş geçilemez!")
                .MinimumLength(3).WithMessage("Hastane adı en az 3 karakter olmalı!")
                .MaximumLength(100).WithMessage("Hastane adı 100 karakteri geçemez!");

            // Şehir Kuralları
            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Şehir seçimi zorunludur!");

            // Kapasite Kuralları
            RuleFor(x => x.MainStorageCapacity)
                .NotEmpty().WithMessage("Depo kapasitesi girilmelidir!")
                .GreaterThan(0).WithMessage("Kapasite 0'dan büyük olmalıdır!")
                .LessThan(1000000).WithMessage("Abartılı kapasite girişi yapılamaz!");

            RuleFor(x => x.WasteStorageCapacity)
               .GreaterThanOrEqualTo(0).WithMessage("Atık kapasitesi negatif olamaz!");
        }
    }
}