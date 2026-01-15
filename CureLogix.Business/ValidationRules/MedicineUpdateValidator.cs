using CureLogix.Entity.DTOs.MedicineDTOs;
using FluentValidation;

namespace CureLogix.Business.ValidationRules
{
    public class MedicineUpdateValidator : AbstractValidator<MedicineUpdateDto>
    {
        public MedicineUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("İlaç adı zorunludur!");
            RuleFor(x => x.ActiveIngredient).NotEmpty().WithMessage("Etken madde zorunludur!");
            RuleFor(x => x.CriticalStockLevel).GreaterThanOrEqualTo(0).WithMessage("Stok negatif olamaz!");
            RuleFor(x => x.ShelfLifeDays).GreaterThan(0).WithMessage("Raf ömrü hatalı!");
        }
    }
}