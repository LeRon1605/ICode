using API.Helper;
using API.Migrations;
using Data.Enum;
using API.Models.DTO;
using API.Repository;
using CodeStudy.Models;
using Data.Entity;
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
            return _userRepository.FindByID(ID);
        }

        public async Task<bool> Remove(string ID)
        {
            User user = _userRepository.FindByID(ID);
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
            User user = _userRepository.FindByID(ID);
            if (user == null)
            {
                return false;
            }
            UserUpdate data = entity as UserUpdate;
            user.Username = (string.IsNullOrEmpty(data.Username)) ? user.Username : data.Username;
            user.Avatar = (string.IsNullOrWhiteSpace(data.UploadImage)) ? user.Avatar : data.UploadImage;
            user.UpdatedAt = DateTime.Now;
            await _unitOfWork.CommitAsync();
            return true;
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

        public IEnumerable<Problem> GetProblemCreatedByUser(string Id, string problemName, string tag)
        {
            return _problemRepository.GetProblemDetailMulti(problem => problem.ArticleID == Id && (string.IsNullOrWhiteSpace(problemName) || problem.Name.Contains(problemName)) && (string.IsNullOrEmpty(tag) || problem.Tags.Any(x => x.ID == tag)));
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

        public async Task<IEnumerable<Problem>> GetProblemSolvedByUser(string Id, string problemName, string tag)
        {
            return await _userRepository.GetProblemSolvedByUser(Id, x => (string.IsNullOrWhiteSpace(problemName) || problemName.Contains(problemName)) && (string.IsNullOrWhiteSpace(tag) || x.Tags.Any(x => x.ID == tag)));
        }
    }
}
