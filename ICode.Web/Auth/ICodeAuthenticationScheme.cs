using CodeStudy.Models;
using ICode.Web.Models.DTO;
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
                Claim[] claims = GetClaims(Request.Cookies["access_token"]);

                ClaimsIdentity identity = new ClaimsIdentity("ICode");
                identity.AddClaims(claims);
                return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name));
            }
            else
            {
                if (Request.Cookies.ContainsKey("refresh_token"))
                {
                    HttpContent body = new StringContent(JsonConvert.SerializeObject(new Token
                    {
                        AccessToken = Request.Cookies["access_token"],
                        RefreshToken = Request.Cookies["refresh_token"],
                    }), Encoding.UTF8, "application/json");
                    response = await _client.PostAsync("/auth/refresh-token", body);
                    if (response.IsSuccessStatusCode)
                    {
                        AuthCredential token = JsonConvert.DeserializeObject<AuthCredential>(await response.Content.ReadAsStringAsync());
                        Response.Cookies.Delete("access_token");
                        Response.Cookies.Delete("refresh_token");
                        Response.Cookies.Append("access_token", token.access_token);
                        Response.Cookies.Append("refresh_token", token.refresh_token);
                        Claim[] claims = GetClaims(token.access_token);
                        ClaimsIdentity identity = new ClaimsIdentity("ICode");
                        identity.AddClaims(claims);
                        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name));
                    }
                }
            }
            return AuthenticateResult.Fail("Invalid token");
        }

        private Claim[] GetClaims(string access_token)
        {
            JwtSecurityToken token = new JwtSecurityToken(jwtEncodedString: access_token);
            return new Claim[]
            {
                    new Claim(ClaimTypes.NameIdentifier, token.Claims.FirstOrDefault(x => x.Type == "ID").Value),
                    new Claim(ClaimTypes.Name, token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value),
                    new Claim(ClaimTypes.Role, token.Claims.FirstOrDefault(x => x.Type == "Role").Value)
            };

        }
    }
}
