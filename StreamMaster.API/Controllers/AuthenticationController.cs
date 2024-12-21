using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Enums;
using StreamMaster.Infrastructure.Authentication;

using IAuthenticationService = StreamMaster.Infrastructure.Authentication.IAuthenticationService;

namespace StreamMaster.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authService, IDataRefreshService dataRefreshService, IOptionsMonitor<Setting> settings) : Controller
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginResource resource, [FromQuery] string? ReturnUrl = null)
        {
            User? user = authService.Login(HttpContext.Request, resource.Username, resource.Password);

            if (user == null)
            {
                return Redirect($"~/login?returnUrl={ReturnUrl}&loginFailed=true");
            }

            List<Claim> claims =
            [
                new Claim("user", user.Username),
                new Claim("identifier", user.Identifier.ToString()),
                new Claim("AuthType", nameof(AuthenticationType.Forms))
            ];

            AuthenticationProperties authProperties = new()
            {
                IsPersistent = resource.RememberMe == "on"
            };

            await HttpContext.SignInAsync(nameof(AuthenticationType.Forms), new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "identifier")), authProperties);
            if (string.IsNullOrEmpty(ReturnUrl))
            {
                ReturnUrl = "/";
            }
            return Redirect(settings.CurrentValue.UrlBase + ReturnUrl);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            authService.Logout(HttpContext);
            await HttpContext.SignOutAsync(nameof(AuthenticationType.Forms));
            await dataRefreshService.AuthLogOut();
            return Redirect(settings.CurrentValue.UrlBase + "/");
        }

        [HttpGet("needAuth")]
        public IActionResult NeedAuth()
        {
            if (settings.CurrentValue.AuthenticationMethod == "None")
            {
                return Ok(false);
            }
            bool a = HttpContext.User?.Identity?.IsAuthenticated == true;
            if (a)
            {
                //hey now
                return Ok(false);
            }

            return Ok(true);
        }
    }
}