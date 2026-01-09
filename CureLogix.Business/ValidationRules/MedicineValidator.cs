using CureLogix.Entity.DTOs.MedicineDTOs;
using FluentValidation;

namespace CureLogix.Business.ValidationRules
{
    public class MedicineValidator : AbstractValidator<MedicineAddDto>
    {
        public MedicineValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("İlaç adı zorunludur!");
            RuleFor(x => x.ActiveIngredient).NotEmpty().WithMessage("Etken madde belirtilmelidir!");
            RuleFor(x => x.Unit).NotEmpty().WithMessage("Birim (Kutu/Şişe) seçilmelidir!");

            RuleFor(x => x.ShelfLifeDays)
                .GreaterThan(0).WithMessage("Raf ömrü 0 günden fazla olmalıdır!");

            RuleFor(x => x.CriticalStockLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Kritik stok negatif olamaz!");
        }
    }
}