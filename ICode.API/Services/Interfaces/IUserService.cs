using CodeStudy.Models;
using Data.Entity;
using ICode.Models;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService: IService<User>
    {
        bool Exist(string username, string email);
        UserDetail GetDetailById(string id);
        User FindByName(string name);
        Task RemindAbsent();
        Task<User> Login(string name, string password, IAuth auth);
        Task<bool> UpdateRole(User user, string role);
        Task<bool> ChangePassword(User user, string token, string password);
        IEnumerable<UserDTO> GetUsersByFilter(string name, bool? gender, DateTime? date, string sort, string orderBy);
        Task<PagingList<UserDTO>> GetPageByFilter(int page, int pageSize, string name, bool? gender, DateTime? date, string sort, string orderBy);
        Task<PagingList<SubmissionDTO>> GetPageSubmitOfUser(int page, int pageSize, string Id, string problem, string language, bool? status, DateTime? date, string sort, string orderBy);
        IEnumerable<SubmissionDTO> GetSubmitOfUser(string Id, string problem, string language, bool? status, DateTime? date, string sort, string orderBy);
        IEnumerable<ProblemDTO> GetProblemCreatedByUser(string Id, string problemName, string tag, DateTime? date, string sort, string orderBy);
        Task<IEnumerable<ProblemDTO>> GetProblemSolvedByUser(string Id, string problemName, string tag);
    }
}
