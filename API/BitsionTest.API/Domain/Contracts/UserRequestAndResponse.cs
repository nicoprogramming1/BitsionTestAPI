namespace BitsionTest.API.Domain.Contracts
{
    public class UserRegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }


    public class UserLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CurrentUserResponse
    {
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AccessToken { get; set; }
    }

    public class RevokeRefreshTokenResponse
    {
        public string Message { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }
}
