using Data.Common;
using CodeStudy.Models;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Services.Interfaces;
using Data.Repository.Interfaces;
using Data.Repository;
using Models.DTO;

namespace Services
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

        public IEnumerable<ProblemDTO> GetProblemCreatedByUser(string Id, string problemName, string tag, DateTime? date, string sort, string orderBy)
        {
            IEnumerable<ProblemDTO> problems = _mapper.Map<IEnumerable<Problem>, IEnumerable<ProblemDTO>>(_problemRepository.GetProblemDetailMulti(problem => problem.ArticleID == Id && problem.Name.Contains(problemName) && (tag == "" || problem.Tags.Any(x => x.Name.Contains(tag))) && (date == null || ((DateTime)date).Date == problem.CreatedAt.Date)));
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort)
                {
                    case "problem":
                        return (orderBy == "asc") ? problems.OrderBy(x => x.Name) : problems.OrderByDescending(x => x.Name);
                    case "date":
                        return (orderBy == "asc") ? problems.OrderBy(x => x.CreatedAt) : problems.OrderByDescending(x => x.CreatedAt);
                    default:
                        throw new Exception("Invalid action.");
                }
            }
            return problems;
        }

        public IEnumerable<SubmissionDTO> GetSubmitOfUser(string Id, string problem, string language, bool? status, DateTime? date, string sort, string orderBy)
        {
            return _mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_submissionRepository.GetSubmissionsDetail(x => x.UserID == Id && x.SubmissionDetails.First().TestCase.Problem.Name.Contains(problem) && x.Language.Contains(language) && (status == null || (bool)status == x.Status) && (date == null || ((DateTime)date).Date == x.CreatedAt.Date)));
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
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort.ToLower())
                {
                    case "name":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.Username) : list.Data.OrderByDescending(x => x.Username);
                        break;
                    case "gender":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.Gender) : list.Data.OrderByDescending(x => x.Gender);
                        break;
                    case "date":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.CreatedAt) : list.Data.OrderByDescending(x => x.CreatedAt);
                        break;
                    default:
                        throw new Exception("Invalid Action.");
                }
            }
            return _mapper.Map<PagingList<User>, PagingList<UserDTO>>(list);
        }

        public async Task<PagingList<SubmissionDTO>> GetPageSubmitOfUser(int page, int pageSize, string Id, string problem, string language, bool? status, DateTime? date, string sort, string orderBy)
        {
            PagingList<SubmissionDTO> list = _mapper.Map<PagingList<Submission>, PagingList<SubmissionDTO>>(await _submissionRepository.GetPageAsync(page, pageSize, x => x.UserID == Id && x.SubmissionDetails.First().TestCase.Problem.Name.Contains(problem) && x.Language.Contains(language) && (status == null || (bool)status == x.Status) && (date == null || ((DateTime)date).Date == x.CreatedAt.Date), x => x.SubmissionDetails, x => x.User));
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort.ToLower())
                {
                    case "user":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.User.Username) : list.Data.OrderByDescending(x => x.User.Username);
                        break;
                    case "problem":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.Problem.Name) : list.Data.OrderByDescending(x => x.Problem.Name);
                        break;
                    case "language":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.Language) : list.Data.OrderByDescending(x => x.Language);
                        break;
                    case "status":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.Status) : list.Data.OrderByDescending(x => x.Status);
                        break;
                    case "date":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.CreatedAt) : list.Data.OrderByDescending(x => x.CreatedAt);
                        break;
                    default:
                        throw new Exception("Invalid Action.");
                }
            }
            return list;
        }
    }
}
