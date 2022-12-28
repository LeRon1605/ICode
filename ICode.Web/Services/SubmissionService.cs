using CodeStudy.Models;
using ICode.Web.Models.DTO;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ICode.Web.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SubmissionService(IHttpClientFactory httpFactory, IHttpContextAccessor httpContextAccessor)
        {
            _client = httpFactory.CreateClient("ICode");
            _httpContextAccessor = httpContextAccessor; 
        }

        public async Task<ServiceResponse<SubmissionResponse>> GetById(string id)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext.Request.Cookies["access_token"]);
            HttpResponseMessage response = await _client.GetAsync($"/submissions/{id}");
            if (response.IsSuccessStatusCode)
            {
                return new ServiceResponse<SubmissionResponse>
                {
                    State = true,
                    Data = JsonConvert.DeserializeObject<SubmissionResponse>(await response.Content.ReadAsStringAsync())
                };
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ServiceResponse<SubmissionResponse>
                {
                    State = false,
                    Data = null
                };
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return new ServiceResponse<SubmissionResponse>
                {
                    State = false,
                    Data = null,
                    Description = "Bạn không có quyền truy cập tài nguyên này"
                };
            }
            return null;
        }

        public async Task<List<SubmissionDTO>> GetSubmissionOfProblem(string problemId, string userId)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext.Request.Cookies["access_token"]);
            HttpResponseMessage response = await _client.GetAsync($"/problems/{problemId}/submissions");
            if (response.IsSuccessStatusCode)
            {
                List<SubmissionDTO> submissions = JsonConvert.DeserializeObject<List<SubmissionDTO>>(await response.Content.ReadAsStringAsync());
                if (userId != null)
                {
                    return submissions.Where(submission => submission.User.ID == userId).ToList();
                }
                return submissions;
            }
            return null;
        }

        public async Task<SubmissionResult> Submit(string id, SubmissionInput submission)
        {
            HttpContent body = new StringContent(JsonConvert.SerializeObject(submission), Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext.Request.Cookies["access_token"]);
            HttpResponseMessage response = await _client.PostAsync($"/problems/{id}/submissions", body);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<SubmissionResult>(await response.Content.ReadAsStringAsync());
            }
            return null;
        }
    }
}
