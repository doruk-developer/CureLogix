namespace CureLogix.Entity.DTOs.HospitalDTOs
{
    public class HospitalAddDto
    {
        public string Name { get; set; }
        public string City { get; set; }
        public int MainStorageCapacity { get; set; }
        public int WasteStorageCapacity { get; set; }
    }
}