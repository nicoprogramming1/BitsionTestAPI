using AutoMapper;
using BitsionTest.API.Domain.Contracts;
using BitsionTest.API.Domain.Entities;
using BitsionTest.API.Repositories.Interface;
using BitsionTest.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace BitsionTest.API.Services.Implementation
{
    public class UserServiceImpl : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserServiceImpl> _logger;
        private readonly IServiceProvider _serviceProvider;

        public UserServiceImpl(ITokenService tokenService, ICurrentUserService currentUserService, IUserRepository userRepository, IMapper mapper, ILogger<UserServiceImpl> logger, IServiceProvider serviceProvider)
        {
            _tokenService = tokenService;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<UserResponse> RegisterAsync(UserRegisterRequest request)
        {
            _logger.LogInformation("Registrando usuario");
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogError("El email ya está registrado");
                throw new Exception("El email ya está registrado");
            }

            var newUser = _mapper.Map<ApplicationUser>(request);
            newUser.UserName = request.Email;

            var createdUser = await _userRepository.CreateUserAsync(newUser, request.Password);
            _logger.LogInformation("Usuario creado con éxito");

            await _tokenService.GenerateToken(createdUser);
            createdUser.CreatedAt = DateTime.Now;

            // Asignamos el rol User por default al momento del registro
            var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(createdUser.Email);
            if (user != null && !await userManager.IsInRoleAsync(user, "User"))
            {
                await userManager.AddToRoleAsync(user, "User");
            }

            return _mapper.Map<UserResponse>(createdUser);
        }


        public async Task<UserResponse> LoginAsync(UserLoginRequest request)
        {
            if (request == null)
            {
                _logger.LogError("No se ha enviado la información necesaria para iniciar sesión");
                throw new ArgumentNullException(nameof(request));
            }

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !await _userRepository.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogError("El email o la contraseña son inválidos");
                throw new Exception("El email o la contraseña son inválidos");
            }

            var token = await _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
            user.RefreshToken = Convert.ToBase64String(refreshTokenHash);
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(2);

            user.CreatedAt = DateTime.Now;
            var result = await _userRepository.UpdateUserAsync(user);

            var userResponse = _mapper.Map<ApplicationUser, UserResponse>(user);
            userResponse.AccessToken = token;
            userResponse.RefreshToken = refreshToken;

            return userResponse;
        }

        public async Task<UserResponse> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Recuperando usuario por id");
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogError("Usuario no encontrado");
                throw new Exception("Usuario no encontrado");
            }
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<CurrentUserResponse> GetCurrentUserAsync()
        {
            _logger.LogInformation("Recuperando usuario actual");
            var user = await _userRepository.GetByIdAsync(Guid.Parse(_currentUserService.GetUserId()));
            if (user == null)
            {
                _logger.LogError("El usuario no fue encontrado");
                throw new Exception("El usuario no fue encontrado");
            }
            return _mapper.Map<CurrentUserResponse>(user);
        }

        public async Task<CurrentUserResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            _logger.LogInformation("RefreshToken");

            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken));
            var hashedRefreshToken = Convert.ToBase64String(refreshTokenHash);

            var user = await _userRepository.GetByRefreshTokenAsync(hashedRefreshToken);
            if (user == null)
            {
                _logger.LogError("Refresh token inválido");
                throw new Exception("Refresh token inválido");
            }

            if (user.RefreshTokenExpiryTime < DateTime.Now)
            {
                _logger.LogWarning("Refresh token expirado para usuario: {UserId}", user.Id);
                throw new Exception("Refresh token expirado");
            }

            var newAccessToken = await _tokenService.GenerateToken(user);
            var currentUserResponse = _mapper.Map<CurrentUserResponse>(user);
            currentUserResponse.AccessToken = newAccessToken;
            return currentUserResponse;
        }

        public async Task<RevokeRefreshTokenResponse> RevokeRefreshToken(RefreshTokenRequest refreshTokenRemoveRequest)
        {
            _logger.LogInformation("Revocando el refresh token");

            try
            {
                using var sha256 = SHA256.Create();
                var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshTokenRemoveRequest.RefreshToken));
                var hashedRefreshToken = Convert.ToBase64String(refreshTokenHash);

                var user = await _userRepository.GetByRefreshTokenAsync(hashedRefreshToken);
                if (user == null)
                {
                    _logger.LogError("Refresh token inválido");
                    throw new Exception("Refresh token inválido");
                }

                if (user.RefreshTokenExpiryTime < DateTime.Now)
                {
                    _logger.LogWarning("El refresh token está expirado para el usuario: {UserId}", user.Id);
                    throw new Exception("Refresh token expirado");
                }

                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;

                var result = await _userRepository.UpdateUserAsync(user);
                if (!result)
                {
                    _logger.LogError("Error al actualizar usuario");
                    return new RevokeRefreshTokenResponse { Message = "Error al revocar el refresh token" };
                }

                _logger.LogInformation("Refresh token revocado con éxito");
                return new RevokeRefreshTokenResponse { Message = "Refresh token revocado con éxito" };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al revocar el refresh token: {ex}", ex.Message);
                throw new Exception("Error al revocar el refresh token");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogError("Usuario no encontrado");
                throw new Exception("Usuario no encontrado");
            }
            await _userRepository.DeleteUserAsync(id);
        }
    }
}
