using System.ComponentModel.DataAnnotations;

namespace CureLogix.WebUI.Models
{
    public class SettingsViewModel
    {
        // Şifre Değiştirme Alanları
        [Required(ErrorMessage = "Mevcut şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifre zorunludur.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Şifreler uyuşmuyor.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        // Görünüm Tercihleri (View'a taşımak için)
        public string? ActiveTheme { get; set; }        // Dark / Light
        public string? ActiveChart { get; set; }        // Bar / Line
        public string? ActiveSidebarColor { get; set; } // Primary, Danger, Success vb.
    }
}