using CodeStudy.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Web.Services
{
    public interface IProblemService
    {
        Task<List<ProblemDTO>> GetAllAsync();
        Task<ProblemDTO> GetByIDAsync(string ID);
    }
    public class ProblemService : IProblemService
    {
        private HttpClient _client { get; set; }
        public ProblemService(HttpClient client)
        {
            _client = client;
        }
        public async Task<List<ProblemDTO>> GetAllAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5001/problems");
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ProblemDTO>>(stream);
            }
            else
            {
                return null;
            }
        }
        public async Task<ProblemDTO> GetByIDAsync(string ID)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5001/problems/" + ID);
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ProblemDTO>(stream);
            }
            else
            {
                return null;
            }
        }
    }
}
