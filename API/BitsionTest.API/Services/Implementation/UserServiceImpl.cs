using AutoMapper;
using BitsionTest.API.Domain.Contracts;
using BitsionTest.API.Domain.Entities;
using BitsionTest.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BitsionTest.API.Services.Implementation
{
    public class UserServiceImpl : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserServiceImpl> _logger;

        // Inicializa una nueva instancia de UserServiceImpl
        public UserServiceImpl(ITokenService tokenService, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<UserServiceImpl> logger)
        {
            _tokenService = tokenService; // el servicio para generar el token
            _currentUserService = currentUserService; // el servicio del usuario actual para recuperar su info
            _userManager = userManager; // gestiona la info del user
            _mapper = mapper; // el mapper a objetos
            _logger = logger;
        }

        // Crea un nuevo usuario validando que el email sea único
        public async Task<UserResponse> RegisterAsync(UserRegisterRequest request)
        {
            _logger.LogInformation("Registrando usuario");
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogError("El email ya está registrado");
                throw new Exception("El email ya está registrado");
            }

            var newUser = _mapper.Map<ApplicationUser>(request);

            // Configuramos manualmente el UserName como el email
            newUser.UserName = request.Email;

            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Error al crear el usuario: {errors}", errors);
                throw new Exception($"Error al crear el usuario: {errors}");
            }
            _logger.LogInformation("Usuario creado con éxito");
            await _tokenService.GenerateToken(newUser);
            newUser.CreatedAt = DateTime.Now;
            return _mapper.Map<UserResponse>(newUser);
        }


        // Maneja el login arrojando excepciones por datos invalidos o faltantes, crea los tokens y actualiza esta info en db
        public async Task<UserResponse> LoginAsync(UserLoginRequest request)
        {
            if (request == null)
            {
                _logger.LogError("No se ha enviado la información necesaria para iniciar sesión");
                throw new ArgumentNullException(nameof(request));
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogError("El email o la contraseña son inválidos");
                throw new Exception("El email o la contraseña son inválidos");
            }

            var token = await _tokenService.GenerateToken(user);

            var refreshToken = _tokenService.GenerateRefreshToken();

            // Hashea el token y lo guarda o sobreescribe en db
            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
            user.RefreshToken = Convert.ToBase64String(refreshTokenHash);
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(2);

            user.CreatedAt = DateTime.Now;

            // Actualiza esta nueva info del token del user en db
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Error al actualizar el usuario: {errors}", errors);
                throw new Exception($"Error al actualizar el usuario: {errors}");
            }

            var userResponse = _mapper.Map<ApplicationUser, UserResponse>(user);
            userResponse.AccessToken = token;
            userResponse.RefreshToken = refreshToken;

            return userResponse;
        }

        // Recupera un usuario por su id
        public async Task<UserResponse> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Recuperando usuario por id");
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogError("Usuario no encontrado");
                throw new Exception("Usuario no encontrado");
            }
            _logger.LogInformation("Usuario no encontrado");
            return _mapper.Map<UserResponse>(user);
        }

        // Recupera el usuario actual
        public async Task<CurrentUserResponse> GetCurrentUserAsync()
        {
            _logger.LogInformation("Recuperando usuario actual");
            var user = await _userManager.FindByIdAsync(_currentUserService.GetUserId());
            if (user == null)
            {
                _logger.LogError("El usuario no fue encontrado");
                throw new Exception("El usuario no fue encontrado");
            }
            return _mapper.Map<CurrentUserResponse>(user);
        }

        // Usando el refresh token actualiza el access token
        public async Task<CurrentUserResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            _logger.LogInformation("RefreshToken");

            // Hashea el refresh token entrante y lo compara con el de db
            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken));
            var hashedRefreshToken = Convert.ToBase64String(refreshTokenHash);

            // Encuentra el usuario segun su refresh token
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == hashedRefreshToken);
            if (user == null)
            {
                _logger.LogError("Refresh token inválido");
                throw new Exception("Refresh token inválido");
            }

            // Valida la expiración del token
            if (user.RefreshTokenExpiryTime < DateTime.Now)
            {
                _logger.LogWarning("Refresh token expirado para usuario: {UserId}", user.Id);
                throw new Exception("Refresh token expirado");
            }

            // Genera el nuevo access token
            var newAccessToken = await _tokenService.GenerateToken(user);
            _logger.LogInformation("Access token generado de forma exitosa");
            var currentUserResponse = _mapper.Map<CurrentUserResponse>(user);
            currentUserResponse.AccessToken = newAccessToken;
            return currentUserResponse;
        }

        // Revoca el refresh token del usuario
        public async Task<RevokeRefreshTokenResponse> RevokeRefreshToken(RefreshTokenRequest refreshTokenRemoveRequest)
        {
            _logger.LogInformation("Revocando el refresh token");

            try
            {
                // Hashea el refresh token
                using var sha256 = SHA256.Create();
                var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshTokenRemoveRequest.RefreshToken));
                var hashedRefreshToken = Convert.ToBase64String(refreshTokenHash);

                // Encuentra el usuario según su refresh token
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == hashedRefreshToken);
                if (user == null)
                {
                    _logger.LogError("Refresh token inválido");
                    throw new Exception("Refresh token inválido");
                }

                // Valida la expiración
                if (user.RefreshTokenExpiryTime < DateTime.Now)
                {
                    _logger.LogWarning("El refresh token está expirado para el usuario: {UserId}", user.Id);
                    throw new Exception("Refresh token expirado");
                }

                // Remover el refresh token
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;

                // Y actualizamos al user en db
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Error al actualizar usuario");
                    return new RevokeRefreshTokenResponse
                    {
                        Message = "Error al revocar el refresh token"
                    };
                }
                _logger.LogInformation("Refresh token revocado con éxito");
                return new RevokeRefreshTokenResponse
                {
                    Message = "Refresh token revocado con éxito"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al revocar el refresh token: {ex}", ex.Message);
                throw new Exception("Error al revocar el refresh token");
            }
        }

        // Eliminar un usuario
        public async Task DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogError("Usuario no encontrado");
                throw new Exception("Usuario no encontrado");
            }
            await _userManager.DeleteAsync(user);
        }
    }
}
