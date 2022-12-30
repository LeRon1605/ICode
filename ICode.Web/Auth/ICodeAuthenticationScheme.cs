using CodeStudy.Models;
using ICode.Web.Models.DTO;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ICode.Web.Auth
{
    public class ICodeAuthSchemeOptions : AuthenticationSchemeOptions
    { }

    public class ICodeAuthenticationScheme : AuthenticationHandler<ICodeAuthSchemeOptions>
    {
        private readonly IAuthService _authService;
        public ICodeAuthenticationScheme(IAuthService authService, IOptionsMonitor<ICodeAuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _authService = authService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Cookies.ContainsKey("access_token"))
            {
                return AuthenticateResult.Fail("Token not found!");
            }

            UserDTO user = await _authService.GetProfile(Request.Cookies["access_token"]);
            if (user != null)
            {
                return AuthenticateResult.Success(GetTicket(Request.Cookies["access_token"], user));
            }
            else
            {
                if (Request.Cookies.ContainsKey("refresh_token"))
                {
                    AuthCredential token = await _authService.RefreshToken(Request.Cookies["access_token"], Request.Cookies["refresh_token"]);
                    if (token != null)
                    {
                        Response.Cookies.Append("access_token", token.access_token);
                        Response.Cookies.Append("refresh_token", token.refresh_token);
                        user = await _authService.GetProfile(token.access_token);
                        return AuthenticateResult.Success(GetTicket(token.access_token, user));
                    }
                }
            }
            return AuthenticateResult.Fail("Invalid token");
        }

        private AuthenticationTicket GetTicket(string access_token, UserDTO user)
        {
            JwtSecurityToken token = new JwtSecurityToken(jwtEncodedString: access_token);
            Claim[] claims = new Claim[]
            {
                    new Claim(ClaimTypes.NameIdentifier, token.Claims.FirstOrDefault(x => x.Type == "ID").Value),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("Avatar", user.Avatar),
                    new Claim(ClaimTypes.Role, token.Claims.FirstOrDefault(x => x.Type == "Role").Value)
            };
            ClaimsIdentity identity = new ClaimsIdentity("ICode");
            identity.AddClaims(claims);
            return new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        }
    }
}
