using CodeStudy.Models;
using ICode.Models.Comment;
using ICode.Web.Services.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ICode.Web.Services
{
    public class CommentService : ICommentService
    {
        private readonly HttpClient _client;
        public CommentService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("ICode");
        }

        public async Task<List<CommentDetail>> GetCommentsOfProblem(string problemId)
        {
            string response = await _client.GetStringAsync($"/problems/{problemId}/comments");
            return JsonConvert.DeserializeObject<List<CommentDetail>>(response);
        }
    }
}
