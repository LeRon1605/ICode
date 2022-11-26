using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Data.Repository;
using Data.Repository.Interfaces;
using ICode.API.Mapper.ContestMapper;
using ICode.Common;
using ICode.Data.Repository.Interfaces;
using ICode.Models;
using ICode.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using Models.DTO;
using Services;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICode.Services
{
    public class ContestService : IContestService
    {
        private readonly IUserRepository _userRepository;
        private readonly IContestRepository _contestRepository;
        private readonly IProblemRepository _problemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mail;
        private readonly IMapper _mapper;
        public ContestService(IContestRepository contestRepository, IUserRepository userRepository, IProblemRepository problemRepository, IUnitOfWork unitOfWork, IMailService mail, IMapper mapper)
        {
            _contestRepository = contestRepository;
            _userRepository = userRepository;
            _problemRepository = problemRepository;
            _unitOfWork = unitOfWork;
            _mail = mail;
            _mapper = mapper;
        }
        public async Task Add(Contest entity)
        {
            await _contestRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task<ServiceResult> Register(string id, string userId)
        {
            Contest contest = _contestRepository.GetContestWithPlayer(x => x.ID == id);
            if (contest == null)
            {
                return new ServiceResult
                {
                    State = ServiceState.EntityNotFound,
                    Message = "Contest doesn't exist",
                    Data = null,
                };
            }
            if (contest.ContestDetails.Count() >= contest.PlayerLimit)
            {
                return new ServiceResult
                {
                    State = ServiceState.InvalidAction,
                    Message = "Contest is full",
                    Data = null,
                };
            }
            if (contest.StartAt <= DateTime.Now)
            {
                return new ServiceResult
                {
                    State = ServiceState.InvalidAction,
                    Message = "Contest has already started or has been ended.",
                    Data = null
                };
            }
            if (contest.ContestDetails.FirstOrDefault(x => x.UserID == userId) != null)
            {
                return new ServiceResult
                {
                    State = ServiceState.InvalidAction,
                    Message = "You has already registered to this contest.",
                    Data = null
                };
            }
            User user = _userRepository.FindByID(userId);
            if (user == null)
            {
                return new ServiceResult
                {
                    State = ServiceState.EntityNotFound,
                    Message = "User doesn't exist",
                    Data = null,
                };
            }
            contest.ContestDetails.Add(new ContestDetail
            {
                UserID = userId,
                RegisteredAt = DateTime.Now,
                Score = 0
            });
            _contestRepository.Update(contest);
            await _unitOfWork.CommitAsync();
            return new ServiceResult
            {
                State = ServiceState.Success,
                Message = "Register user successfully",
                Data = null,
            };
        }

        public Contest FindByID(string ID)
        {
            return _contestRepository.FindByID(ID); 
        }

        public IEnumerable<Contest> GetAll()
        {
            return _contestRepository.FindAll();
        }

        public List<ContestBase> GetContestByFilter(string name, DateTime? date, bool? state, string sort, string orderBy, IContestMapper contestMapper)
        {
            List<ContestBase> contests = _contestRepository.GetContestWithProblemMulti(x => x.Name.Contains(name) && (state == null || ((bool)state == (x.StartAt <= DateTime.Now && DateTime.Now <= x.EndAt))) && (date == null || x.StartAt.Date == date.Value.Date)).Select(x => contestMapper.Map(x)).ToList();
            if (string.IsNullOrWhiteSpace(sort))
            {
                switch (sort)
                {
                    case "name":
                        return (orderBy == "asc") ? contests.OrderBy(x => x.Name).ToList() : contests.OrderByDescending(x => x.Name).ToList();
                    case "date":
                        return (orderBy == "asc") ? contests.OrderBy(x => x.StartAt).ToList() : contests.OrderByDescending(x => x.StartAt).ToList();
                    case "state":
                        return (orderBy == "asc") ? contests.OrderBy(x => x.State).ToList() : contests.OrderByDescending(x => x.State).ToList();
                };
            }
            return contests;
        }

        public ContestBase GetDetailById(string id, IContestMapper contestMapper)
        {
            Contest contest = _contestRepository.GetContestWithProblem(x => x.ID == id);
            if (contest == null)
            {
                return null;
            }
            return contestMapper.Map(contest);
        }

        public PagingList<ContestBase> GetPageContestByFilter(int page, int pageSize, string name, DateTime? date, bool? state, string sort, string orderBy, IContestMapper contestMapper)
        {
            List<ContestBase> contests = _contestRepository.GetContestWithProblemMulti(x => x.Name.Contains(name) && (state == null || ((bool)state == (x.StartAt <= DateTime.Now && DateTime.Now <= x.EndAt))) && (date == null || x.StartAt.Date == date.Value.Date)).Select(x => contestMapper.Map(x)).ToList();
            PagingList<ContestBase> list = new PagingList<ContestBase>()
            {
                TotalPage = (int)Math.Ceiling((float)contests.Count / pageSize),
                Data = contests,
                Page = page
            };
            if (string.IsNullOrWhiteSpace(sort))
            {
                switch (sort)
                {
                    case "name":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.Name).ToList() : list.Data.OrderByDescending(x => x.Name).ToList();
                        break;
                    case "date":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.StartAt).ToList() : list.Data.OrderByDescending(x => x.StartAt).ToList();
                        break;
                    case "state":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.State).ToList() : list.Data.OrderByDescending(x => x.State).ToList();
                        break;
                };
            }
            return list;
        }

        public List<UserContest> GetPlayerOfContest(string id, string name, bool? gender, DateTime? registeredAt, string sort, string orderBy)
        {
            List<UserContest> users = _mapper.Map<List<ContestDetail>, List<UserContest>>(_contestRepository.GetContestWithPlayer(x => x.ID == id).ContestDetails.Where(x => x.User.Username.Contains(name) && (gender == null || (bool)gender == x.User.Gender) && (registeredAt == null || registeredAt.Value.Date == x.RegisteredAt.Date)).ToList());
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort)
                {
                    case "name":
                        return (orderBy == "asc") ? users.OrderBy(x => x.User.Username).ToList() : users.OrderByDescending(x => x.User.Username).ToList();
                    case "gender":
                        return (orderBy == "asc") ? users.OrderBy(x => x.User.Gender).ToList() : users.OrderByDescending(x => x.User.Gender).ToList();
                    case "date":
                        return (orderBy == "asc") ? users.OrderBy(x => x.RegisteredAt).ToList() : users.OrderByDescending(x => x.RegisteredAt).ToList();
                }
            }
            return users;
        }

        public PagingList<UserContest> GetPagePlayerOfContestByFilter(string id, int page, int pageSize, string name, bool? gender, DateTime? registeredAt, string sort, string orderBy)
        {
            List<UserContest> users = _mapper.Map<List<ContestDetail>, List<UserContest>>(_contestRepository.GetContestWithPlayer(x => x.ID == id).ContestDetails.Where(x => x.User.Username.Contains(name) && (gender == null || (bool)gender == x.User.Gender) && (registeredAt == null || registeredAt.Value.Date == x.RegisteredAt.Date)).ToList());
            PagingList<UserContest> list = new PagingList<UserContest>()
            {
                TotalPage = (int)Math.Ceiling((float)users.Count / pageSize),
                Data = users,
                Page = page
            };
            if (string.IsNullOrWhiteSpace(sort))
            {
                switch (sort)
                {
                    case "name":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.User.Username).ToList() : list.Data.OrderByDescending(x => x.User.Username).ToList();
                        break;
                    case "gender":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.User.Gender).ToList() : list.Data.OrderByDescending(x => x.User.Gender).ToList();
                        break;
                    case "date":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.RegisteredAt).ToList() : list.Data.OrderByDescending(x => x.RegisteredAt).ToList();
                        break;
                };
            }
            return list;
        }

        public async Task NotifyUser(string id)
        {
            Contest contest = _contestRepository.FindByID(id);
            if (contest != null)
            {
                List<Task> tasks = new List<Task>();
                foreach (User user in _userRepository.FindAll())
                {
                    tasks.Add(_mail.SendMailAsync(user.Email, "Thông báo contest", $"{contest.Name} sẽ bắt đầu vào {contest.StartAt} với nhiều phần thưởng hấp dẫn"));
                }
                await Task.WhenAll(tasks);
            }
        }

        public async Task<bool> Remove(string ID)
        {
            Contest contest = _contestRepository.FindByID(ID);
            if (contest == null)
            {
                return false;
            }
            _contestRepository.Remove(contest);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> Update(string ID, object entity)
        {
            Contest contest = _contestRepository.GetContestWithProblem(x => x.ID == ID);
            if (contest == null)
            {
                return false;
            }
            else
            {
                ContestUpdate input = entity as ContestUpdate;
                contest.Name = string.IsNullOrWhiteSpace(input.Name) ? contest.Name : input.Name;
                contest.Description = string.IsNullOrWhiteSpace(input.Description) ? contest.Description : input.Description;
                if (input.StartAt != null && input.StartAt < DateTime.Now && ((input.StartAt < contest.EndAt && input.EndAt == null) || (input.StartAt < input.EndAt && input.EndAt != null)))
                {
                    contest.StartAt = input.StartAt.Value;
                }
                if (input.EndAt != null && input.EndAt > DateTime.Now && (input.EndAt > contest.StartAt))
                {
                    contest.EndAt = input.EndAt.Value;
                }
                contest.PlayerLimit = input.PlayerLimit ?? contest.PlayerLimit;
                if (input.Problems != null && input.Problems.Length > 0)
                {
                    string[] invalidProblem = input.Problems.Where(x => _problemRepository.FindByID(x.ID) == null).Select(x => x.ID).ToArray();
                    if (invalidProblem.Length == 0)
                    {
                        contest.ProblemContestDetails = input.Problems.Select(x => new ProblemContestDetail
                        {
                            ProblemID = x.ID,
                            Level = x.Level,
                            Score = x.Score
                        }).ToList();
                    }
                }
                _contestRepository.Update(contest);
                await _unitOfWork.CommitAsync();
                return true;
            }
        }

        public async Task<ServiceResult> RemoveUser(string id, string userId)
        {
            Contest contest = _contestRepository.GetContestWithPlayer(x => x.ID == id);
            if (contest != null)
            {
                if (contest.ContestDetails.FirstOrDefault(x => x.UserID == userId) != null)
                {
                    contest.ContestDetails = contest.ContestDetails.Where(x => x.UserID != userId).ToList();
                    _contestRepository.Update(contest);
                    await _unitOfWork.CommitAsync();
                    return new ServiceResult
                    {
                        State = ServiceState.Success,
                        Data = _mapper.Map<List<ContestDetail>, List<UserContest>>(contest.ContestDetails.ToList()),
                        Message = "Success"
                    };
                }
                else
                {
                    return new ServiceResult
                    {
                        State = ServiceState.InvalidAction,
                        Data = null,
                        Message = "User hasn't registered to this contest yet."
                    };
                }
            }
            else
            {
                return new ServiceResult
                {
                    State = ServiceState.EntityNotFound,
                    Data = null,
                    Message = "Contest doesn't exist."
                };
            }
        }

        public ServiceResult GetSubmissions(string id)
        {
            Contest contest = _contestRepository.FindByID(id);
            if (contest == null)
            {
                return new ServiceResult
                {
                    State = ServiceState.EntityNotFound,
                    Data = null,
                    Message = "Contest doesn't exist."
                };
            }
            return new ServiceResult
            {
                State = ServiceState.Success,
                Data = _mapper.Map<List<Submission>, List<SubmissionDTO>>(_contestRepository.GetContestSubmissionMulti(x => x.ContestSubmission.ContestID == id).ToList())
            };
        }

        public bool IsUserInContest(string id, string userId)
        {
            Contest contest = _contestRepository.GetContestWithPlayer(x => x.ID == id);
            if (contest == null) return false;
            return contest.ContestDetails.FirstOrDefault(x => x.UserID == userId) != null;
        }

        public bool IsUserSolvedProblem(string id, string userId, string problemId)
        {
            Contest contest = _contestRepository.FindByID(id);
            if (contest == null) return false;
            return _contestRepository.GetContestSubmission(x => x.ContestSubmission.ContestID == id && x.SubmissionDetails.First().TestCase.ProblemID == problemId && x.State == SubmitState.Success) != null;
        }

        public bool IsProblemInContest(string id, string problemId)
        {
            Contest contest = _contestRepository.GetContestWithPlayer(x => x.ID == id);
            if (contest == null) return false;
            return _contestRepository.GetProblemInContest(x => x.ID == id).FirstOrDefault(x => x.ProblemID == problemId) != null;
        }

        public async Task<ServiceResult> AddPointForUser(string id, string userId, string problemId)
        {
            Contest contest = _contestRepository.GetContestWithPlayer(x => x.ID == id);
            if (contest == null)
            {
                return new ServiceResult
                {
                    State = ServiceState.EntityNotFound,
                    Data = null,
                    Message = "Contest doesn't exist."
                };
            }
            ContestDetail player = contest.ContestDetails.FirstOrDefault(x => x.UserID == userId);
            if (player == null)
            {
                return new ServiceResult
                {
                    State = ServiceState.EntityNotFound,
                    Data = null,
                    Message = "Player hasn't been registered to the contest yet."
                };
            }
            ProblemContestDetail problem = _contestRepository.GetProblemInContest(x => x.ID == id).FirstOrDefault(x => x.ProblemID == problemId);
            if (problem == null)
            {
                return new ServiceResult
                {
                    State = ServiceState.EntityNotFound,
                    Data = null,
                    Message = "Problem is not in the contest."
                };
            }

            int score = problem.Score;

            player.Score += score;
            _contestRepository.Update(contest);

            await _unitOfWork.CommitAsync();

            return new ServiceResult
            {
                State = ServiceState.Success
            };
        }
    }
}
