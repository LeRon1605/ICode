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
using AutoMapper;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProblemRepository _problemRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IProblemRepository problemRepository,IRoleRepository roleRepository, ISubmissionRepository submissionRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userRepository = userRepository;
            _problemRepository = problemRepository;
            _roleRepository = roleRepository;
            _submissionRepository = submissionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region CRUD
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
        #endregion

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

        public IEnumerable<ProblemDTO> GetProblemCreatedByUser(string Id, string problemName, string tag)
        {
            return _mapper.Map<IEnumerable<Problem>, IEnumerable<ProblemDTO>>(_problemRepository.GetProblemDetailMulti(problem => problem.ArticleID == Id && (string.IsNullOrWhiteSpace(problemName) || problem.Name.Contains(problemName)) && (string.IsNullOrEmpty(tag) || problem.Tags.Any(x => x.ID == tag))));
        }

        public IEnumerable<SubmissionDTO> GetSubmitOfUser(string Id)
        {
            return _mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_submissionRepository.GetSubmissionsDetail(x => x.UserID == Id));
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

        public async Task<IEnumerable<ProblemDTO>> GetProblemSolvedByUser(string Id, string problemName, string tag)
        {
            return _mapper.Map<IEnumerable<Problem>, IEnumerable<ProblemDTO>>(await _userRepository.GetProblemSolvedByUser(Id, x => problemName.Contains(problemName) && (string.IsNullOrWhiteSpace(tag) || x.Tags.Any(x => x.ID == tag))));
        }

        public IEnumerable<UserDTO> GetUsersByFilter(string name, bool? gender, DateTime? date, string sort, string orderBy)
        {
            IEnumerable<UserDTO> users = _mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(_userRepository.FindMulti(x => (x.Username.Contains(name) || x.Email.Contains(name)) && (gender == null || (bool)gender == x.Gender) && (date == null || x.CreatedAt.Date == ((DateTime)date).Date)));
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort.ToLower())
                {
                    case "name":
                        return (orderBy == "asc") ? users.OrderBy(x => x.Username) : users.OrderByDescending(x => x.Username);
                    case "gender":
                        return (orderBy == "asc") ? users.OrderBy(x => x.Gender) : users.OrderByDescending(x => x.Gender);
                    case "date":
                        return (orderBy == "asc") ? users.OrderBy(x => x.CreatedAt) : users.OrderByDescending(x => x.CreatedAt);
                    default:
                        throw new Exception("Invalid Action.");
                }
            }
            return users;
        }

        public async Task<PagingList<UserDTO>> GetPageByFilter(int page, int pageSize, string name, bool? gender, DateTime? date, string sort, string orderBy)
        {
            PagingList<User> list = await _userRepository.GetPageAsync(page, pageSize, x => (x.Username.Contains(name) || x.Email.Contains(name)) && (gender == null || (bool)gender == x.Gender) && (date == null || x.CreatedAt.Date == ((DateTime)date).Date));
            return _mapper.Map<PagingList<User>, PagingList<UserDTO>>(list);
        }

    }
}
