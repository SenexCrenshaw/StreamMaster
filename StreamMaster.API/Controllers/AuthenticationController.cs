using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Enums;
using StreamMaster.Infrastructure.Authentication;

using System.Security.Claims;

using IAuthenticationService = StreamMaster.Infrastructure.Authentication.IAuthenticationService;

namespace StreamMaster.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authService, IOptionsMonitor<Setting> intsettings) : Controller
    {
        private readonly Setting settings = intsettings.CurrentValue;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginResource resource, [FromQuery] string? returnUrl = null)
        {
            User user = await authService.Login(HttpContext.Request, resource.Username, resource.Password);

            if (user == null)
            {
                return Redirect($"~/login?returnUrl={returnUrl}&loginFailed=true");
            }

            List<Claim> claims =
            [
                new Claim("user", user.Username),
                new Claim("identifier", user.Identifier.ToString()),
                new Claim("AuthType", AuthenticationType.Forms.ToString())
            ];

            AuthenticationProperties authProperties = new()
            {
                IsPersistent = resource.RememberMe == "on"
            };

            await HttpContext.SignInAsync(AuthenticationType.Forms.ToString(), new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "identifier")), authProperties);

            return Redirect(settings.UrlBase + "/");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await authService.Logout(HttpContext);
            await HttpContext.SignOutAsync(AuthenticationType.Forms.ToString());
            return Redirect(settings.UrlBase + "/");
        }
    }
}