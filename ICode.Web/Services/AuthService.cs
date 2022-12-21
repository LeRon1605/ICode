using CodeStudy.Models;
using ICode.Web.Models.DTO;
using ICode.Web.Services.Interfaces;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ICode.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _client;
        public AuthService(IHttpClientFactory httpFactory)
        {
            _client = httpFactory.CreateClient("ICode");
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
    }
}
