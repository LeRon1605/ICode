using CodeStudy.Models;
using ICode.Web.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICode.Web.Services.Interfaces
{
    public interface ISubmissionService
    {
        Task<List<SubmissionDTO>> GetSubmissionOfProblem(string problemId, string userId = null);
        Task<SubmissionResult> Submit(string id, SubmissionInput submission);
        Task<ServiceResponse<SubmissionResponse>> GetById(string id);
    }
}
