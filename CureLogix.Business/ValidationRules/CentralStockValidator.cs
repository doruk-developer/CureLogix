using CureLogix.Entity.DTOs.WarehouseDTOs;
using FluentValidation;

namespace CureLogix.Business.ValidationRules
{
    public class CentralStockValidator : AbstractValidator<CentralStockAddDto>
    {
        public CentralStockValidator()
        {
            RuleFor(x => x.MedicineId).NotEmpty().WithMessage("İlaç seçimi zorunludur!");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Giriş miktarı en az 1 olmalıdır!");

            RuleFor(x => x.BatchNo)
                .NotEmpty().WithMessage("Parti (Lot) numarası zorunludur!");

            RuleFor(x => x.ExpiryDate)
                .NotEmpty().WithMessage("Son kullanma tarihi girilmelidir!")
                .GreaterThan(DateTime.Today).WithMessage("Son kullanma tarihi bugünden ileri bir tarih olmalıdır!");
        }
    }
}