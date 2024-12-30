namespace BitsionTest.API.Models.Domain
{
    public class Client
    {
        public Guid Id { get; set; }
        public required string LongName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public bool CanDrive { get; set; }
        public bool WearGlasses { get; set; }
        public bool IsDiabetic { get; set; }
        public bool OtherDiseases { get; set; }
    }
}
