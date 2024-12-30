using BitsionTest.API.Models.Domain.Entities;

namespace BitsionTest.API.Services.Interface
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
        string GenerateRefreshToken();
    }
}
