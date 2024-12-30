namespace BitsionTest.API.Models.Domain.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public required string LongName { get; set; }
        public required int Age { get; set; }
        public required string Gender { get; set; }
        public required string Email { get; set; }
        public string? Nationality { get; set; }
        public required string State { get; set; }
        public required string Phone { get; set; }
        public required bool CanDrive { get; set; }
        public required bool WearGlasses { get; set; }
        public required bool IsDiabetic { get; set; }
        public string? OtherDiseases { get; set; }
    }
}
