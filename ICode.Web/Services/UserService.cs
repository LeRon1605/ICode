using CodeStudy.Models;
using ICode.Web.Models.DTO;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Models;
using Models.Statistic;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ICode.Web.Services
{
    public class UserService: IUserService
    {
        private readonly HttpClient _client;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _client = httpClientFactory.CreateClient("ICode");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ProblemDTO>> GetProblemSolvedByUser()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext.Request.Cookies["access_token"]);
            HttpResponseMessage response = await _client.GetAsync("/me/problems?status=solved");
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<ProblemDTO>>(await response.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<UserDTO> GetProfile()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext.Request.Cookies["access_token"]);
            HttpResponseMessage response = await _client.GetAsync("/me");
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<UserDTO>(await response.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<List<UserRank>> GetUserRank()
        {
            string response = await _client.GetStringAsync("/collection/rank");
            CollectionResponse<List<UserRank>> responseObj = JsonConvert.DeserializeObject<CollectionResponse<List<UserRank>>>(response);
            return responseObj.Data;
        }
    }
}
