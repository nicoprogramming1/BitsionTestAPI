
using BitsionTest.API.Domain.Contracts;
using BitsionTest.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BitsionTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Los captura el "../Exceptions/GlobalExceptionHandler.cs"
            }
            var response = await _userService.RegisterAsync(request);
            return Ok(response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.LoginAsync(request);
            return Ok(response);
        }

        // Get user por una id
        [HttpGet("user/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.GetByIdAsync(id);
            return Ok(response);
        }

        // Actualiza el access token a partir del refresh token
        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.RefreshTokenAsync(request);
            return Ok(response);
        }


        [HttpPost("revoke-refresh-token")]
        [Authorize]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.RevokeRefreshToken(request);
            if (response != null && response.Message == "Refresh token revocado exitosamente")
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var response = await _userService.GetCurrentUserAsync();
            return Ok(response);
        }


        [HttpDelete("user/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("El ID proveído no es válido.");
            }
            await _userService.DeleteAsync(id);
            return Ok();
        }
    }
}