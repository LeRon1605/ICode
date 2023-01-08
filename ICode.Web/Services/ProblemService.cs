using CodeStudy.Models;
using ICode.Web.Extension;
using ICode.Web.Models.DTO;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Models;
using Models.DTO;
using Models.Statistic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ICode.Web.Services
{
    public class ProblemService : IProblemService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProblemService(IHttpClientFactory httpFactory, IHttpContextAccessor httpContextAccessor)
        {
            _client = httpFactory.CreateClient("ICode");
            _httpContextAccessor = httpContextAccessor; 
        }

        public async Task<ProblemDTO> GetById(string id)
        {
            string response = await _client.GetStringAsync($"/problems/{id}");
            return JsonConvert.DeserializeObject<ProblemDTO>(response);
        }

        public async Task<List<ProblemStatistic>> GetHotProblems(int take)
        {
            string response = await _client.GetStringAsync("/collection/hot-problems");
            CollectionResponse<List<ProblemStatistic>> responseObj = JsonConvert.DeserializeObject<CollectionResponse<List<ProblemStatistic>>>(response);
            return responseObj.Data.Take(take).ToList();
        }

        public async Task<List<ProblemDTO>> GetNewProblems(int take)
        {
            string response = await _client.GetStringAsync($"/collection/new-problems?startDate={DateTime.Now.AddDays(-7).Date}");
            CollectionResponse<List<StatisticResponse<List<ProblemDTO>>>> responseObj = JsonConvert.DeserializeObject<CollectionResponse<List<StatisticResponse<List<ProblemDTO>>>>>(response);
            List<ProblemDTO> problems = new List<ProblemDTO>();
            foreach (var item in responseObj.Data)
            {
                if (item.Data.Count > 0)
                {
                    problems.AddRange(item.Data);
                }
            }
            return problems.Take(take).ToList();
        }

        public async Task<List<ProblemDTO>> GetAll(string keyword = "", string tag = "", DateTime? date = null, string sort = "", string orderBy = "")
        {
            QueryBuilder builder = new QueryBuilder();
            builder.AddQuery("name", keyword);
            builder.AddQuery("author", keyword);
            builder.AddQuery("tag", tag);
            builder.AddQuery("date", date);
            builder.AddQuery("sort", sort);
            builder.AddQuery("orderBy", orderBy);
            string response = await _client.GetStringAsync($"/problems{builder}");
            return JsonConvert.DeserializeObject<List<ProblemDTO>>(response);
        }

        public async Task<PagingList<ProblemDTO>> GetPage(int page, int pageSize = 5, string keyword = "", string tag = "", DateTime? date = null, string sort = "", string orderBy = "")
        {
            QueryBuilder builder = new QueryBuilder();
            builder.AddQuery("name", keyword);
            builder.AddQuery("tag", tag);
            builder.AddQuery("date", date);
            builder.AddQuery("sort", sort);
            builder.AddQuery("orderBy", orderBy);
            builder.AddQuery("page", page);
            builder.AddQuery("pageSize", pageSize);
            string response = await _client.GetStringAsync($"/problems{builder}");
            return JsonConvert.DeserializeObject<PagingList<ProblemDTO>>(response);
        }

        public async Task<bool> Add(ProblemInput data)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext.Request.Cookies["access_token"]);
            HttpContent body = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("/problems", body);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Remove(string id)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext.Request.Cookies["access_token"]);
            HttpResponseMessage response = await _client.DeleteAsync($"/problems/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
