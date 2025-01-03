namespace BitsionTest.API.Domain.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public string LongName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string? Nationality { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public bool CanDrive { get; set; }
        public bool WearGlasses { get; set; }
        public bool IsDiabetic { get; set; }
        public string? OtherDiseases { get; set; }
        public bool isDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
