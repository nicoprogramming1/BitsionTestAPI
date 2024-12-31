using BitsionTest.API.Services.Interface;
using System.Security.Claims;

namespace BitsionTest.API.Services.Implementation
{
    /**
     * Esta clase no necesita un repositorio ya que no interactúa con la DB
     * */

    public class CurrentUserServiceImpl : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserServiceImpl(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }
        public string? GetUserId()
        {
            // recuperamos el primer id que haga match con la req
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }
    }
}
