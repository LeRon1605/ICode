using API.Helper;
using API.Models.DTO;
using API.Models.Entity;
using API.Repository;
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

    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Add(RegisterUser input)
        {
            User user = new User
            {
                ID = Guid.NewGuid().ToString(),
                Email = input.Email,
                Password = Encryptor.MD5Hash(input.Password),
                Username = input.Username,
                CreatedAt = DateTime.Now,
                Type = AccountType.Local,
                Role = _roleRepository.FindSingle(role => role.Name == "User")
            };
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task<User> AddGoogle(string email, string name)
        {
            User user = new User
            {
                ID = Guid.NewGuid().ToString(),
                Email = email,
                Password = Encryptor.MD5Hash("password_default"),
                Username = name,
                CreatedAt = DateTime.Now,
                Type = AccountType.Google,
                Role = _roleRepository.FindSingle(role => role.Name == "User")
            };
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();
            return user;
        }

        public async Task<bool> ChangePassword(User user, string token, string password)
        {
            if (user.ForgotPasswordToken != null)
            {
                if (user.ForgotPasswordToken == token && user.ForgotPasswordTokenExpireAt >= DateTime.Now)
                {
                    user.Password = Encryptor.MD5Hash(password);
                    user.ForgotPasswordToken = null;
                    user.ForgotPasswordTokenCreatedAt = null;
                    user.ForgotPasswordTokenExpireAt = null;
                    _userRepository.Update(user);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
            }
            return false;
        }

        public bool Exist(string username, string email)
        {
            return _userRepository.isExist(x => x.Username == username || x.Email == email);
        }

        public User FindById(string id)
        {
            return _userRepository.FindSingle(user => user.ID == id);
        }

        public User FindByName(string name)
        {
            return _userRepository.FindSingle(user => (user.Username == name || user.Email == name) && user.Type == AccountType.Local);
        }
        public User Login(string name, string password, IAuth auth)
        {
            return _userRepository.GetUserWithRole(auth.Login(name, password));
        }
    }
}
