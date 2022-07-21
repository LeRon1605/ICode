using CodeStudy.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Web.Services
{
    public interface ISubmissionService
    {
        Task<SubmissionDTO> SubmitCode(string ProblemID, SubmissionInput input);
    }
    public class SubmissionService: ISubmissionService
    {
        private HttpClient _client { get; set; }
        public SubmissionService(HttpClient client)
        {
            _client = client;
        }

        public async Task<SubmissionDTO> SubmitCode(string ProblemID, SubmissionInput input)
        {
            var result = await _client.PostAsJsonAsync($"/problems/{ProblemID}/submissions", input);
            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<SubmissionDTO>(content);
                return response;
            }
            return null;
        }
    }
}
