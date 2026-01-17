namespace CureLogix.Entity.DTOs.UserDTOs
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string NameSurname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? ProfilePicture { get; set; } // Resim yolu (String)
    }
}