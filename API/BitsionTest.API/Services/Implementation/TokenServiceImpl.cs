using BitsionTest.API.Domain.Contracts;
using BitsionTest.API.Domain.Entities;
using BitsionTest.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BitsionTest.API.Services.Implementation
{
    public class TokenServiceImpl : ITokenService
    {
        private readonly SymmetricSecurityKey _secretKey;
        private readonly string? _validIssuer;
        private readonly string? _validAudience;
        private readonly double _expires;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TokenServiceImpl> _logger;

        public TokenServiceImpl(IConfiguration configuration, UserManager<ApplicationUser> userManager, ILogger<TokenServiceImpl> logger)
        {
            _userManager = userManager;
            _logger = logger;
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new InvalidOperationException("La clave secreta JWT no está configurada.");
            }

            _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            _validIssuer = jwtSettings.ValidIssuer;
            _validAudience = jwtSettings.ValidAudience;
            _expires = jwtSettings.Expires;
        }

        // genera el token para el usuario
        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var singingCredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256);
            var claims = await GetClaimsAsync(user);
            var tokenOptions = GenerateTokenOptions(singingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

       // los claim (reclamaciones) devuelven al usuario determinada información de su identidad
       private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user?.Id ?? string.Empty),
                new Claim(ClaimTypes.Email, user?.Email ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claims;
        }

        // genera las opciones que necesita el JWT
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: _validIssuer,
                audience: _validAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_expires),
                signingCredentials: signingCredentials
            );
        }

        // genera el refresh token y lo devuelve
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var refreshToken = Convert.ToBase64String(randomNumber);
            return refreshToken;
        }
    }
}
