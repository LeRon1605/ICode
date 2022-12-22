using ICode.Web.Models.DTO;
using ICode.Web.Services.Interfaces;
using Models;
using Models.Statistic;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ICode.Web.Services
{
    public class UserService: IUserService
    {
        private readonly HttpClient _client;
        public UserService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("ICode");
        }

        public async Task<List<UserRank>> GetUserRank()
        {
            string response = await _client.GetStringAsync("/collection/rank");
            CollectionResponse<List<UserRank>> responseObj = JsonConvert.DeserializeObject<CollectionResponse<List<UserRank>>>(response);
            return responseObj.Data;
        }
    }
}
