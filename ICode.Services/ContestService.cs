using AutoMapper;
using Data.Entity;
using Data.Repository;
using Data.Repository.Interfaces;
using ICode.Common;
using ICode.Data.Repository.Interfaces;
using ICode.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IMapper _mapper;
        private readonly IMailService _mail;
        public ContestService(IContestRepository contestRepository, IUserRepository userRepository, IProblemRepository problemRepository, IUnitOfWork unitOfWork, IMapper mapper, IMailService mail)
        {
            _contestRepository = contestRepository;
            _userRepository = userRepository;
            _problemRepository = problemRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mail = mail;
        }
        public async Task Add(Contest entity)
        {
            await _contestRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public Contest FindByID(string ID)
        {
            return _contestRepository.FindByID(ID); 
        }

        public IEnumerable<Contest> GetAll()
        {
            return _contestRepository.FindAll();
        }

        public List<ContestBase> GetContestByFilter(string name, DateTime? date, bool? state, string sort, string orderBy)
        {
            List<ContestBase> contests = _contestRepository.GetDetailMulti(x => x.Name.Contains(name) && (state == null || ((bool)state == (x.StartAt <= DateTime.Now && DateTime.Now <= x.EndAt))) && (date == null || x.StartAt.Date == date.Value.Date)).Select(x =>
            {
                if (x.StartAt <= DateTime.Now && DateTime.Now <= x.EndAt)
                {
                    return _mapper.Map<Contest, ContestDTO>(x);
                }
                else
                {
                    return _mapper.Map<Contest, ContestBase>(x);
                }
            }).ToList();
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

        public ContestBase GetDetailById(string id)
        {
            Contest contest = _contestRepository.GetDetailSingle(x => x.ID == id);
            if (contest == null)
            {
                return null;
            }
            if (contest.StartAt <= DateTime.Now && DateTime.Now <= contest.EndAt)
            {
                return _mapper.Map<Contest, ContestDTO>(contest);
            }
            else
            {
                return _mapper.Map<Contest, ContestBase>(contest);
            }
        }

        public PagingList<ContestBase> GetPageContestByFilter(int page, int pageSize, string name, DateTime? date, bool? state, string sort, string orderBy)
        {
            List<ContestBase> contests = _contestRepository.GetDetailMulti(x => x.Name.Contains(name) && (state == null || ((bool)state == (x.StartAt <= DateTime.Now && DateTime.Now <= x.EndAt))) && (date == null || x.StartAt.Date == date.Value.Date)).Select(x =>
            {
                if (x.StartAt <= DateTime.Now && DateTime.Now <= x.EndAt)
                {
                    return _mapper.Map<Contest, ContestDTO>(x);
                }
                else
                {
                    return _mapper.Map<Contest, ContestBase>(x);
                }
            }).ToList();
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
            Contest contest = _contestRepository.GetDetailSingle(x => x.ID == ID);
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
    }
}
