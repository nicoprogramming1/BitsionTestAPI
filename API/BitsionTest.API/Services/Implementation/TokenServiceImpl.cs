using BitsionTest.API.Models.Domain.Entities;
using BitsionTest.API.Services.Interface;

namespace BitsionTest.API.Services.Implementation
{
    public class TokenServiceImpl : ITokenService
    {
        public string GenerateRefreshToken()
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateToken(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}
