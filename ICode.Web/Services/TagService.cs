using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CodeStudy.Models;
using ICode.Web.Services.Interfaces;
using Newtonsoft.Json;

namespace ICode.Web.Services
{
    public class TagService : ITagService
    {
        private readonly HttpClient _client;
        public TagService(IHttpClientFactory httpClientFactory) 
        {
            _client = httpClientFactory.CreateClient("ICode");
        }
        public async Task<List<TagDTO>> GetAll()
        {
            string response = await _client.GetStringAsync("/tags");
            return JsonConvert.DeserializeObject<List<TagDTO>>(response);
        }
    }
}