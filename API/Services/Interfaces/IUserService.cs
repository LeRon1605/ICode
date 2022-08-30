using API.Models.DTO;
using CodeStudy.Models;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IUserService: IService<User>
    {
        bool Exist(string username, string email);
        User FindByName(string name);
        User Login(string name, string password, IAuth auth);
        Task<bool> UpdateRole(User user, string role);
        Task<bool> ChangePassword(User user, string token, string password);
        IEnumerable<UserDTO> GetUsersByFilter(string name, bool? gender, DateTime? date, string sort, string orderBy);
        Task<PagingList<UserDTO>> GetPageByFilter(int page, int pageSize, string name, bool? gender, DateTime? date, string sort, string orderBy);
        IEnumerable<SubmissionDTO> GetSubmitOfUser(string Id);
        IEnumerable<ProblemDTO> GetProblemCreatedByUser(string Id, string problemName, string tag);
        Task<IEnumerable<ProblemDTO>> GetProblemSolvedByUser(string Id, string problemName, string tag);
    }
}
