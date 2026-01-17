using CureLogix.Entity.DTOs.UserDTOs;
using FluentValidation;

namespace CureLogix.Business.ValidationRules
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Ünvan seçimi zorunludur!");
            RuleFor(x => x.NameSurname).NotEmpty().WithMessage("Ad Soyad zorunludur!");
            RuleFor(x => x.Username).NotEmpty().WithMessage("Kullanıcı adı zorunludur!");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Geçerli bir e-posta giriniz!");
        }
    }
}