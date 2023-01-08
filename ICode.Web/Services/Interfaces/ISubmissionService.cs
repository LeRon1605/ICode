using CodeStudy.Models;
using ICode.Web.Models.DTO;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICode.Web.Services.Interfaces
{
    public interface ISubmissionService
    {
        Task<List<SubmissionDTO>> GetAll();
        Task<PagingList<SubmissionDTO>> GetPage(int page, int pageSize, string keyword = "", string language = "", bool? status = null, DateTime? date = null, string sort = "", string orderBy = "asc"); 
        Task<List<SubmissionDTO>> GetSubmissionOfProblem(string problemId, string userId = null);
        Task<PagingList<SubmissionDTO>> GetPageSubmissionsOfProblem(string problemId, int page, int pageSize, string keyword = "", string language = "", bool? status = null, DateTime? date = null, string sort = "", string orderBy = "asc");
        Task<PagingList<SubmissionDTO>> GetPageSubmissionsOfUser(string userId, int page, int pageSize, string keyword = "", string language = "", bool? status = null, DateTime? date = null, string sort = "", string orderBy = "asc");
        Task<SubmissionResult> Submit(string id, SubmissionInput submission);
        Task<ServiceResponse<SubmissionResponse>> GetById(string id);
    }
}
