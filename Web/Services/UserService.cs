using CodeStudy.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Web.Services
{
    public interface IUserService
    {
        Task<bool> Register(RegisterUser input);
    }
    public class UserService : IUserService
    {
        private HttpClient _client { get; set; }
        public UserService(HttpClient client)
        {
            _client = client;
        }
        public async Task<bool> Register(RegisterUser input)
        {
            string body = JsonConvert.SerializeObject(input);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/auth/register")
            {
                Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await _client.SendAsync(request);
            return (response.StatusCode == System.Net.HttpStatusCode.Created);
        }
    }
}
