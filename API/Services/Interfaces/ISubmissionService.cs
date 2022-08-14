using API.Models.DTO;
using API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ISubmissionService
    {
        Task<Submission> Submit(Submission submission, string problemID);
        Submission FindById(string Id);
        IEnumerable<Submission> FindAll();
        IEnumerable<SubmissionDetail> GetDetail(string Id);
        IEnumerable<Submission> GetSubmissionsOfProblem(string problemId);
        IEnumerable<Submission> GetSubmissionOfUsers(string userId);
        Task Remove(Submission submission);
        Task<PagingList<Submission>> GetPageAsync(int page, int pageSize, bool? status, string keyword);
    }
}
