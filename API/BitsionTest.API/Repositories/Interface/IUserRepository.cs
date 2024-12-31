using BitsionTest.API.Domain.Entities;

namespace BitsionTest.API.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetByIdAsync(Guid id);
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<ApplicationUser> GetByRefreshTokenAsync(string refreshToken);
        Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> DeleteUserAsync(Guid id);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task AddUserToRoleAsync(ApplicationUser user, string role);
    }
}
