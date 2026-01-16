using CureLogix.Entity.DTOs.UserDTOs;
using FluentValidation;

namespace CureLogix.Business.ValidationRules
{
    // SINIF İSMİNİ DEĞİŞTİRDİK: UserValidator -> AppUserValidator
    public class AppUserValidator : AbstractValidator<UserAddDto>
    {
        public AppUserValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Ünvan seçimi zorunludur!");
            RuleFor(x => x.NameSurname).NotEmpty().WithMessage("Ad Soyad zorunludur!");
            RuleFor(x => x.Username).NotEmpty().WithMessage("Kullanıcı adı zorunludur!");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Geçerli bir e-posta giriniz!");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz!")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalı!");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Şifreler uyuşmuyor!");
        }
    }
}