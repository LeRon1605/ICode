using API.Models.DTO;
using API.Models.Entity;
using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IUserService
    {
        bool Exist(string username, string email);
        User Login(string name, string password, IAuth auth);
        Task Add(RegisterUser input);
        Task<User> AddGoogle(string email, string name);
        User FindByName(string name);
        User FindById(string id);
        Task<bool> ChangePassword(User user, string token, string password);
        IEnumerable<User> GetAll();
        Task<PagingList<User>> GetPageAsync(int page, int pageSize, string keyword);
        Task<bool> Update(User user, UserUpdate input);
        Task Remove(User user);
        Task<bool> UpdateRole(User user, string role);
        IEnumerable<Submission> GetSubmitOfUser(string Id);
    }
}
