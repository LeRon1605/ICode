using API.Models.DTO;
using CloudinaryDotNet;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ISubmissionService: IService<Submission>
    {
        Task<Submission> Submit(Submission submission, string problemID);
        IEnumerable<SubmissionDetail> GetDetail(string Id);
        IEnumerable<Submission> GetSubmissionsOfProblem(string problemId);
        IEnumerable<Submission> GetSubmissionOfUsers(string userId, bool? status = null);
        Task<PagingList<Submission>> GetPageAsync(int page, int pageSize, bool? status, string user);
    }
}
