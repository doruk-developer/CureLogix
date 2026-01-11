using Microsoft.AspNetCore.Identity;

namespace CureLogix.Entity.Concrete
{
    // Primary Key'i 'int' yapıyoruz (Varsayılan Guid'dir, int daha performanslıdır)
    public class AppUser : IdentityUser<int>
    {
        public string NameSurname { get; set; }
    }
}