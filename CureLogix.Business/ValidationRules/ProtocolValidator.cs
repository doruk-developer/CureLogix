using CureLogix.Entity.DTOs.ProtocolDTOs;
using FluentValidation;

namespace CureLogix.Business.ValidationRules
{
    public class ProtocolValidator : AbstractValidator<ProtocolCreateDto>
    {
        public ProtocolValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Protokol başlığı zorunludur!");

            RuleFor(x => x.DiseaseId)
                .NotEmpty().WithMessage("Hastalık seçimi zorunludur!");

            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("Doktor seçimi zorunludur!");

            // LİSTE KONTROLÜ (Collection Validator)
            RuleFor(x => x.Medicines)
                .NotEmpty().WithMessage("Reçeteye en az bir ilaç eklemelisiniz!")
                .Must(x => x.Count > 0).WithMessage("Reçete boş olamaz!");

            // Listenin içindeki her bir eleman için kural
            RuleForEach(x => x.Medicines).ChildRules(medicine =>
            {
                medicine.RuleFor(m => m.MedicineId).NotEmpty().WithMessage("İlaç seçilmedi!");
                medicine.RuleFor(m => m.RequiredQuantity).GreaterThan(0).WithMessage("Adet 0 olamaz!");
                medicine.RuleFor(m => m.DosageInstructions).NotEmpty().WithMessage("Dozaj tarifi giriniz!");
            });
        }
    }
}