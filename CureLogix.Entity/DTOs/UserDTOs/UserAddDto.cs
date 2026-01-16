using Microsoft.AspNetCore.Http; // IFormFile için gerekli

namespace CureLogix.Entity.DTOs.UserDTOs
{
    public class UserAddDto
    {
        public string? Title { get; set; }
        public string? NameSurname { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        // Formdan gelen fiziksel dosya
        public IFormFile? Photo { get; set; }
    }
}