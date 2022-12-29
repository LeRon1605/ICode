using CodeStudy.Models;
using ICode.Web.Models.DTO;
using ICode.Web.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ICode.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _client;
        private readonly HttpClient _googleOathClient;
        private readonly IConfiguration _configuration;
        public AuthService(IHttpClientFactory httpFactory, IConfiguration configuration)
        {
            _client = httpFactory.CreateClient("ICode");
            _googleOathClient = httpFactory.CreateClient("GoogleOAuth");
            _configuration = configuration;
        }

        public async Task<AuthCredential> Login(LoginUser user)
        {
            HttpContent body = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("/auth/login", body);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<AuthCredential>(await response.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<bool> Register(RegisterUser user)
        {
            HttpContent body = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("/auth/register", body);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RequestChangePassword(ForgetPassword data)
        {
            HttpContent body = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("/auth/forget-password", body);
            return response.IsSuccessStatusCode;
        }

        public async Task<ServiceResponse<bool>> ChangePassword(string userId, string token, ForgetPasswordSubmit data)
        {
            HttpContent body = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"/auth/forget-password/callback?userId={userId}&token={token}", body);
            if (response.IsSuccessStatusCode)
            {
                return new ServiceResponse<bool>
                {
                    State = true,
                    Data = true
                };
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new ServiceResponse<bool>
                {
                    State = false,
                    Description = "Người dùng không tồn tại"
                };
            }
            else
            {
                return new ServiceResponse<bool>
                {
                    State = false,
                    Description = "Token không hợp lệ"
                };
            }
        }

        public async Task<AuthCredential> LoginByGoogle(string access_token)
        {
            HttpContent body = new StringContent(JsonConvert.SerializeObject(new { AccessToken = access_token }), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"/auth/google-signin", body);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<AuthCredential>(await response.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<GoogleTokenResponse> GetGoogleToken(string code)
        {
            HttpContent body = new StringContent(JsonConvert.SerializeObject(new
            {
                client_id = _configuration["Oauth:Google:ClientID"],
                client_secret = _configuration["Oauth:Google:ClientSecret"],
                code = code,
                grant_type = "authorization_code",
                redirect_uri = _configuration["Oauth:Google:RedirectURL"]
            }), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _googleOathClient.PostAsync("/token", body);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<GoogleTokenResponse>(await response.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<UserDTO> GetProfile(string access_token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await _client.GetAsync("/me");
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<UserDTO>(await response.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<AuthCredential> RefreshToken(string access_token, string refresh_token)
        {
            HttpContent body = new StringContent(JsonConvert.SerializeObject(new Token
            {
                AccessToken = access_token,
                RefreshToken = refresh_token,
            }), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("/auth/refresh-token", body);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<AuthCredential>(await response.Content.ReadAsStringAsync());
            }
            return null;
        }
    }
}
