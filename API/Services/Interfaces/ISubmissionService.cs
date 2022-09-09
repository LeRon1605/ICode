using API.Models.DTO;
using CloudinaryDotNet;
using CodeStudy.Models;
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
        Task<SubmissionDTO> Submit(Submission submission, string problemID);
        SubmissionDTO GetDetail(string Id);
        IEnumerable<SubmissionDetailDTO> GetSubmitDetail(string Id);
        IEnumerable<SubmissionDTO> GetSubmissionByFilter(string user, string problem, string language, bool? status, DateTime? date, string sort, string orderBy);
        IEnumerable<SubmissionDTO> GetSubmissionsOfProblem(string problemId);
        Task<PagingList<SubmissionDTO>> GetPageByFilter(int page, int pageSize, string user, string problem, string language, bool? status, DateTime? date, string sort, string orderBy);
    }
}
