using CureLogix.Entity.Enums; // Enum kullanıyorsan bunu ekle, yoksa int kalsın

namespace CureLogix.Entity.DTOs.DoctorDTOs
{
    public class DoctorAddDto
    {
        // 1. Stringlerin yanına '?' koyduk.
        // Artık .NET hata vermeyecek, null olarak geçirecek.
        // Hatayı DoctorValidator yakalayıp "Ad Soyad zorunludur!" diyecek.
        public string? FullName { get; set; }

        public string? Title { get; set; }

        public string? Specialty { get; set; }

        // 2. 'int' yerine 'int?' veya 'RoleTypeEnum?' yaptık.
        // Artık "Seçiniz" gelince sistem patlamaz, null olur.
        // Hatayı Validator yakalar: "Sistem rolü seçilmelidir."
        public int? RoleType { get; set; }
        // Veya projenin yapısına göre: public RoleTypeEnum? RoleType { get; set; }

        // Bu zaten doğruydu ama dursun
        public int? HospitalId { get; set; }

        public string? Email { get; set; }
    }
}