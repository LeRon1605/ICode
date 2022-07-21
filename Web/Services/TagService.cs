using CodeStudy.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Web.Services
{
    public interface ITagService
    {
        Task<List<TagDTO>> GetAll();
    }
    public class TagService: ITagService
    {
        private HttpClient _client { get; set; }
        public TagService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<TagDTO>> GetAll()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5001/tags");
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TagDTO>>(stream);
            }
            else
            {
                return null;
            }
        }
    }
}
