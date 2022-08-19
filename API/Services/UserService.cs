using API.Helper;
using API.Migrations;
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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProblemRepository _problemRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;
        public UserService(IUserRepository userRepository, IProblemRepository problemRepository,IRoleRepository roleRepository, ISubmissionRepository submissionRepository,IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _problemRepository = problemRepository;
            _roleRepository = roleRepository;
            _submissionRepository = submissionRepository;
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<User> GetAll()
        {
            return _userRepository.FindAll();
        }

        public async Task Add(User entity)
        {
            await _userRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public User FindByID(string ID)
        {
            return _userRepository.FindSingle(user => user.ID == ID);
        }

        public async Task<bool> Remove(string ID)
        {
            User user = _userRepository.FindSingle(user => user.ID == ID);
            if (user == null)
            {
                return false;
            }
            _userRepository.Remove(user);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> Update(string ID, object entity)
        {
            User user = _userRepository.FindSingle(user => user.ID == ID);
            if (user == null)
            {
                return false;
            }
            UserUpdate data = entity as UserUpdate;
            user.Username = (string.IsNullOrEmpty(data.Username)) ? user.Username : data.Username;
            user.UpdatedAt = DateTime.Now;
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<User> AddGoogle(string email, string name)
        {
            User user = new User
            {
                ID = Guid.NewGuid().ToString(),
                Email = email,
                Password = Encryptor.MD5Hash(Constant.PASSWORD_DEFAULT),
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

        public async Task<PagingList<User>> GetPageAsync(int page, int pageSize, string keyword)
        {
            return await _userRepository.GetPageAsync(page, pageSize, user => user.Username.Contains(keyword) || user.Email.Contains(keyword));
        }

        public IEnumerable<Problem> GetProblemCreatedByUser(string Id)
        {
            return _problemRepository.FindMulti(problem => problem.ArticleID == Id);
        }

        public IEnumerable<Submission> GetSubmitOfUser(string Id)
        {
            return _submissionRepository.GetSubmissionsDetail(x => x.UserID == Id);
        }

        public User Login(string name, string password, IAuth auth)
        {
            return _userRepository.GetUserWithRole(auth.Login(name, password));
        }

        public async Task<bool> UpdateRole(User user, string role)
        {
            Role entity = _roleRepository.findByName(role);
            if (entity == null)
            {
                return false;
            }
            else
            {
                user.RoleID = entity.ID;
                _userRepository.Update(user);
                await _unitOfWork.CommitAsync();
                return true;
            }
        }
    }
}
