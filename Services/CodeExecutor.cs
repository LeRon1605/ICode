using Microsoft.Extensions.Configuration;
using Models.DTO;
using Newtonsoft.Json;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CodeExecutor : ICodeExecutor
    {
        private readonly IConfiguration _configuration;

        public CodeExecutor(IConfiguration configuration)
        {
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
            HttpClient client = new HttpClient();
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
