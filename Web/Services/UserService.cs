using Blazored.LocalStorage;
using CodeStudy.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Services
{
    public interface IUserService
    {
        Task<bool> Register(RegisterUser input);
        Task<LoginResponse> Login(LoginUser user);
        Task Logout();
        Task<Response<UserDTO>> FindByID(string ID);
        Task<Response<UserDTO>> UpdateUser(string ID, UserUpdate input);
    }
    public class UserService : IUserService
    {
        private readonly HttpClient _client;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        public UserService(HttpClient client, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            _client = client;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
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

        public async Task<LoginResponse> Login(LoginUser user)
        {
            var result = await _client.PostAsJsonAsync("/auth/login", user);
            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<LoginResponse>(content);
                await _localStorage.SetItemAsync("authToken", response.token);
                (_authenticationStateProvider as ApiAuthenticationProvider).MarkUserAsAuthenticated(response.token);
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", response.token);
                return response;
            }
            return new LoginResponse { status = false };
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            (_authenticationStateProvider as ApiAuthenticationProvider).MarkUserAsLoggedOut();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "");
        }

        public async Task<Response<UserDTO>> FindByID(string ID)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5001/users/" + ID);
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStringAsync();
                return new Response<UserDTO>
                {
                    StatusCode = response.StatusCode,
                    Data = JsonConvert.DeserializeObject<UserDTO>(stream)
                };
            }
            return new Response<UserDTO>
            {
                StatusCode = response.StatusCode,
                Data = null
            };
        }

        public async Task<Response<UserDTO>> UpdateUser(string ID, UserUpdate input)
        {
            var result = await _client.PutAsJsonAsync($"/users/{ID}", input);
            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                return new Response<UserDTO>
                {
                    StatusCode = result.StatusCode,
                    Data = JsonConvert.DeserializeObject<UserDTO>(content)
                };
            }
            return new Response<UserDTO>
            {
                StatusCode = result.StatusCode,
                Data = null
            };
        }
    }
}
