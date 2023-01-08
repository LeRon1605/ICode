using CodeStudy.Models;
using Data.Entity;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISubmissionService: IService<Submission>
    {
        Task<SubmissionResult> Submit(Submission submission, string problemID);
        SubmissionDTO GetDetail(string Id);
        IEnumerable<SubmissionDetailDTO> GetSubmitDetail(string Id);
        IEnumerable<SubmissionDTO> GetSubmissionByFilter(string user, string problem, string language, bool? status, DateTime? date, string sort, string orderBy);
        IEnumerable<SubmissionDTO> GetSubmissionsOfProblem(string problemId);
        Task<PagingList<SubmissionDTO>> GetPageSubmissionsOfProblem(string problemId, int page, int pageSize, string user, string language, bool? status, DateTime? date, string sort, string orderBy);
        Task<PagingList<SubmissionDTO>> GetPageByFilter(int page, int pageSize, string user, string problem, string language, bool? status, DateTime? date, string sort, string orderBy);
    }
}
