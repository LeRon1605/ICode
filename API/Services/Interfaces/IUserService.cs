using API.Models.DTO;
using API.Models.Entity;
using CodeStudy.Models;
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
        Task<User> AddGoogle(string email, string name);
        Task<bool> ChangePassword(User user, string token, string password);
        Task<PagingList<User>> GetPageAsync(int page, int pageSize, string keyword);
        Task<bool> UpdateRole(User user, string role);
        IEnumerable<Submission> GetSubmitOfUser(string Id);
        IEnumerable<Problem> GetProblemCreatedByUser(string Id, string problemName, string tag);
        Task<IEnumerable<Problem>> GetProblemSolvedByUser(string Id, string problemName, string tag);
    }
}
