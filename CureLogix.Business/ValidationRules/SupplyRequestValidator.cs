using CureLogix.Entity.DTOs.SupplyDTOs;
using FluentValidation;

namespace CureLogix.Business.ValidationRules
{
    public class SupplyRequestValidator : AbstractValidator<SupplyRequestAddDto>
    {
        public SupplyRequestValidator()
        {
            RuleFor(x => x.HospitalId).NotEmpty().WithMessage("Hastane seçilmelidir.");
            RuleFor(x => x.MedicineId).NotEmpty().WithMessage("İlaç seçilmelidir.");
            RuleFor(x => x.RequestQuantity).GreaterThan(0).WithMessage("Talep miktarı en az 1 olmalıdır.");
        }
    }
}