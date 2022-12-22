using CodeStudy.Models;
using ICode.Web.Models.DTO;
using ICode.Web.Services.Interfaces;
using Models;
using Models.Statistic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ICode.Web.Services
{
    public class ProblemService : IProblemService
    {
        private readonly HttpClient _client;
        public ProblemService(IHttpClientFactory httpFactory)
        {
            _client = httpFactory.CreateClient("ICode");
        }

        public async Task<ProblemDTO> GetById(string id)
        {
            string response = await _client.GetStringAsync($"/problems/{id}");
            return JsonConvert.DeserializeObject<ProblemDTO>(response);
        }

        public async Task<List<ProblemDTO>> GetAll()
        {
            string response = await _client.GetStringAsync("/problems");
            return JsonConvert.DeserializeObject<List<ProblemDTO>>(response);
        }

        public async Task<List<ProblemStatistic>> GetHotProblems()
        {
            string response = await _client.GetStringAsync("/collection/hot-problems");
            CollectionResponse<List<ProblemStatistic>> responseObj = JsonConvert.DeserializeObject<CollectionResponse<List<ProblemStatistic>>>(response);
            return responseObj.Data;
        }

        public async Task<List<ProblemDTO>> GetNewProblems()
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
            return problems;
        }
    }
}
