using Microsoft.AspNetCore.Identity;

namespace CureLogix.Entity.Concrete
{
    // Primary Key'i 'int' yapıyoruz (Varsayılan Guid'dir, int daha performanslıdır)
    public class AppUser : IdentityUser<int>
    {
        public string NameSurname { get; set; }

        // SQL'e eklediğimiz kullanıcı profil kolonun karşılığı
        public string? ProfilePicture { get; set; }

        // YENİ EKLENEN: Ünvan
        public string? Title { get; set; }
    }
}