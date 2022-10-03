using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Data.Repository;
using Data.Repository.Interfaces;
using Models.DTO;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReplyRepository _replyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ReportService(IReportRepository reportRepository, IReplyRepository replyRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _replyRepository = replyRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region CRUD
        public IEnumerable<Report> GetAll()
        {
            return _reportRepository.GetReportsDetail();
        }

        public async Task Add(Report report)
        {
            await _reportRepository.AddAsync(report);
            await _unitOfWork.CommitAsync();
        }

        public Report FindByID(string ID)
        {
            return _reportRepository.FindByID(ID);
        }

        public async Task<bool> Remove(string ID)
        {
            Report report = _reportRepository.FindByID(ID);
            if (report == null)
            {
                return false;
            }
            _reportRepository.Remove(report);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> Update(string ID, object entity)
        {
            Report report = _reportRepository.FindByID(ID);
            if (report == null)
            {
                return false;
            }
            ReportInput data = entity as ReportInput;
            report.Title = (string.IsNullOrWhiteSpace(data.Title)) ? report.Title : data.Title;
            report.Content = (string.IsNullOrWhiteSpace(data.Content)) ? report.Content : data.Content;
            report.UpdatedAt = DateTime.Now;
            _reportRepository.Update(report);
            await _unitOfWork.CommitAsync();
            return true;
        }
        #endregion

        public async Task<bool> Reply(string reportId, ReplyInput input)
        {
            Report report = _reportRepository.GetReportsDetailSingle(x => x.ID == reportId);
            if (report.Reply == null)
            {
                report.Reply = new Reply
                {
                    Content = input.Content,
                    CreatedAt = DateTime.Now,
                };
                _reportRepository.Update(report);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateReply(string reportId, ReplyInput input)
        {
            Report report = _reportRepository.GetReportsDetailSingle(x => x.ID == reportId);
            if (report.Reply == null)
            {
                return false;
            }
            else
            {
                report.Reply.Content = input.Content;
                report.Reply.UpdatedAt = DateTime.Now;
                _reportRepository.Update(report);
                await _unitOfWork.CommitAsync();
                return true;
            }
        }

        public async Task<bool> RemoveReply(string reportId)
        {
            Report report = _reportRepository.GetReportsDetailSingle(x => x.ID == reportId);
            if (report.Reply == null)
            {
                return false;
            }
            else
            {
                _replyRepository.Remove(report.Reply);
                await _unitOfWork.CommitAsync();
                return true;
            }
        }

        public ReportDTO GetDetailById(string Id)
        {
            return _mapper.Map<Report, ReportDTO>(_reportRepository.GetReportsDetailSingle(x => x.ID == Id));
        }

        public IEnumerable<ReportDTO> GetReportByFilter(string title, string user, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy)
        {
            IEnumerable<ReportDTO> data = _mapper.Map<IEnumerable<Report>, IEnumerable<ReportDTO>>(_reportRepository.GetReportsDetail(x => x.Title.Contains(title) && x.User.Username.Contains(user) && x.Problem.Name.Contains(problem) && (createdAt == null || x.CreatedAt.Date == ((DateTime)createdAt).Date) && (reply == null || (x.Reply != null) == reply)));
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort) 
                {
                    case "title":
                        return (orderBy == "asc") ? data.OrderBy(x => x.Title) : data.OrderByDescending(x => x.Title);
                    case "user":
                        return (orderBy == "asc") ? data.OrderBy(x => x.User.Username) : data.OrderByDescending(x => x.User.Username);
                    case "problem":
                        return (orderBy == "asc") ? data.OrderBy(x => x.Problem.Name) : data.OrderByDescending(x => x.Problem.Name);
                    case "date":
                        return (orderBy == "asc") ? data.OrderBy(x => x.CreatedAt) : data.OrderByDescending(x => x.CreatedAt);
                    case "reply":
                        return (orderBy == "asc") ? data.OrderBy(x => x.CreatedAt) : data.OrderByDescending(x => x.CreatedAt);
                    default:
                        throw new Exception("Invalid Action");
                }
            }
            return data;
        }

        public IEnumerable<ReportDTO> GetReportOfUserByFilter(string title, string userId, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy)
        {
            IEnumerable<ReportDTO> data = _mapper.Map<IEnumerable<Report>, IEnumerable<ReportDTO>>(_reportRepository.GetReportsDetail(x => x.Title.Contains(title) && x.UserID == userId && x.Problem.Name.Contains(problem) && (createdAt == null || x.CreatedAt.Date == ((DateTime)createdAt).Date) && (reply == null || (x.Reply != null) == reply)));
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "title":
                        return (orderBy == "asc") ? data.OrderBy(x => x.Title) : data.OrderByDescending(x => x.Title);
                    case "user":
                        return (orderBy == "asc") ? data.OrderBy(x => x.User.Username) : data.OrderByDescending(x => x.User.Username);
                    case "problem":
                        return (orderBy == "asc") ? data.OrderBy(x => x.Problem.Name) : data.OrderByDescending(x => x.Problem.Name);
                    case "date":
                        return (orderBy == "asc") ? data.OrderBy(x => x.CreatedAt) : data.OrderByDescending(x => x.CreatedAt);
                    case "reply":
                        return (orderBy == "asc") ? data.OrderBy(x => x.CreatedAt) : data.OrderByDescending(x => x.CreatedAt);
                    default:
                        throw new Exception("Invalid Action");
                }
            }
            return data;
        }

        public async Task<PagingList<ReportDTO>> GetPageAsync(int page, int pageSize, string title, string user, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy)
        {
            PagingList<Report> list = await _reportRepository.GetPageAsync(page, pageSize, x => x.Title.Contains(title) && x.User.Username.Contains(user) && x.Problem.Name.Contains(problem) && (createdAt == null || x.CreatedAt.Date == ((DateTime)createdAt).Date) && (reply == null || (x.Reply != null) == reply), x => x.User, x => x.Problem, x => x.Reply);
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "title":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.Title) : list.Data.OrderByDescending(x => x.Title);
                        break;
                    case "user":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.User.Username) : list.Data.OrderByDescending(x => x.User.Username);
                        break;
                    case "problem":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.Problem.Name) : list.Data.OrderByDescending(x => x.Problem.Name);
                        break;
                    case "date":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.CreatedAt) : list.Data.OrderByDescending(x => x.CreatedAt);
                        break;
                    case "reply":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.CreatedAt) : list.Data.OrderByDescending(x => x.CreatedAt);
                        break;
                    default:
                        throw new Exception("Invalid Action");
                }
            }
            return _mapper.Map<PagingList<Report>, PagingList<ReportDTO>>(list);
        }

        public async Task<PagingList<ReportDTO>> GetPageReportOfUser(int page, int pageSize, string title, string userId, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy)
        {
            PagingList<Report> list = await _reportRepository.GetPageAsync(page, pageSize, x => x.UserID == userId && x.Title.Contains(title) && x.Problem.Name.Contains(problem) && (createdAt == null || x.CreatedAt.Date == ((DateTime)createdAt).Date) && (reply == null || (x.Reply != null) == reply), x => x.User, x => x.Problem, x => x.Reply);
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "title":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.Title) : list.Data.OrderByDescending(x => x.Title);
                        break;
                    case "user":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.User.Username) : list.Data.OrderByDescending(x => x.User.Username);
                        break;
                    case "problem":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.Problem.Name) : list.Data.OrderByDescending(x => x.Problem.Name);
                        break;
                    case "date":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.CreatedAt) : list.Data.OrderByDescending(x => x.CreatedAt);
                        break;
                    case "reply":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.CreatedAt) : list.Data.OrderByDescending(x => x.CreatedAt);
                        break;
                    default:
                        throw new Exception("Invalid Action");
                }
            }
            return _mapper.Map<PagingList<Report>, PagingList<ReportDTO>>(list);
        }
    }
}
