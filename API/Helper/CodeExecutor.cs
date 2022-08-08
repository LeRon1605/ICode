using API.Models.DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.Helper
{
    public interface ICodeExecutor
    {
        Task<ExecutorResult> ExecuteCode(string code, string language, string input);
    }
    public class CodeExecutor : ICodeExecutor
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CodeExecutor(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<ExecutorResult> ExecuteCode(string code, string language, string input)
        {
            string body = JsonConvert.SerializeObject(new
            {
                script = code,
                language = language,
                versionIndex = "5",
                stdin = input,
                clientId = _configuration["JDoodle:ClientID"],
                clientSecret = _configuration["JDoodle:SecretKey"]
            });
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.jdoodle.com/execute")
            {
                Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json"),
            };
            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ExecutorResult>(res);
            }
            else
            {
                return null;
            }
        }
    }
}
