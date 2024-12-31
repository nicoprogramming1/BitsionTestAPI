using BitsionTest.API.Domain.Entities;
using BitsionTest.API.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BitsionTest.API.Repositories.Implementation
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepositoryImpl(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> GetByIdAsync(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<ApplicationUser> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new Exception($"Error al crear el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            return user;
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task AddUserToRoleAsync(ApplicationUser user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                throw new Exception($"Error al agregar al rol: {role}. {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        /*
         * Este método elimina el rol de un usuario
         * No será implementado en esta prueba técnica
         * 
        public async Task RemoveUserFromRoleAsync(ApplicationUser user, string role)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded)
            {
                throw new Exception($"Error al quitar al rol: {role}. {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        */

        public async Task<ApplicationUser> GetByRefreshTokenHashAsync(string refreshTokenHash)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenHash);
        }
    }
}
