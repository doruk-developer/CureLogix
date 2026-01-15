using CureLogix.Entity.DTOs.DiseaseDTOs;
using FluentValidation;

namespace CureLogix.Business.ValidationRules
{
    public class DiseaseUpdateValidator : AbstractValidator<DiseaseUpdateDto>
    {
        public DiseaseUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Hastalık adı zorunludur!");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("ICD Kodu boş geçilemez!")
                .MaximumLength(10).WithMessage("ICD Kodu en fazla 10 karakter olabilir!");

            RuleFor(x => x.RiskLevel).NotEmpty().WithMessage("Risk seviyesi seçilmelidir!");
        }
    }
}