namespace BitsionTest.API.Models.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required Role Role { get; set; }
    }
}
