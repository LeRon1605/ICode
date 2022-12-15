using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ICode.Web.Auth
{
    public class ICodeAuthSchemeOptions : AuthenticationSchemeOptions
    { }
    
    public class ICodeAuthenticationScheme : AuthenticationHandler<ICodeAuthSchemeOptions>
    {
        public ICodeAuthenticationScheme(Microsoft.Extensions.Options.IOptionsMonitor<ICodeAuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Cookies.ContainsKey("access_token"))
            {
                return AuthenticateResult.Fail("Token not found!");
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:5001");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["access_token"]);
            HttpResponseMessage response = await client.GetAsync("/auth");
            if (response.IsSuccessStatusCode)
            {
                JwtSecurityToken token = new JwtSecurityToken(jwtEncodedString: Request.Cookies["access_token"]);
                Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, token.Claims.FirstOrDefault(x => x.Type == "ID").Value),
                    new Claim(ClaimTypes.Name, token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value),
                    new Claim(ClaimTypes.Role, token.Claims.FirstOrDefault(x => x.Type == "Role").Value)
                };
                return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims)), Scheme.Name));
            }
            return AuthenticateResult.Fail("Invalid token");
        }
    }
}
