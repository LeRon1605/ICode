using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ICode.Web.Auth
{
    public class ICodeAuthSchemeOptions : AuthenticationSchemeOptions
    { }

    public class ICodeAuthenticationScheme : AuthenticationHandler<ICodeAuthSchemeOptions>
    {
        private readonly HttpClient _client;
        public ICodeAuthenticationScheme(IHttpClientFactory httpClientFactory, IOptionsMonitor<ICodeAuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _client = httpClientFactory.CreateClient("ICode");
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Cookies.ContainsKey("access_token"))
            {
                return AuthenticateResult.Fail("Token not found!");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["access_token"]);
            HttpResponseMessage response = await _client.GetAsync("/auth");
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
