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
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReplyRepository _replyRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ReportService(IReportRepository reportRepository, IReplyRepository replyRepository, IUnitOfWork unitOfWork)
        {
            _reportRepository = reportRepository;
            _replyRepository = replyRepository;
            _unitOfWork = unitOfWork;
        }

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

        public async Task<PagingList<Report>> GetReportsOfUser(int page, int pageSize, string userID, string problem)
        {
            return await _reportRepository.GetPageAsync(page, pageSize, x => x.UserID == userID && x.Problem.Name.Contains(problem), x => x.User, x => x.Problem, x => x.Reply);
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
            report.Content = (string.IsNullOrWhiteSpace(report.Content)) ? report.Content : data.Content;
            report.UpdatedAt = DateTime.Now;
            _reportRepository.Update(report);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> Reply(Report report, ReplyInput input)
        {
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

        public async Task<bool> UpdateReply(Report report, ReplyInput input)
        {
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

        public async Task<bool> RemoveReply(Report report)
        {
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

        public async Task<PagingList<Report>> GetPageAsync(int page, int pageSize, string title, string user, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy)
        {
            return await _reportRepository.GetPageAsync(page, pageSize, x => x.Title.Contains(title) && x.User.Username.Contains(user) && x.Problem.Name.Contains(problem) && (createdAt == null || x.CreatedAt.Date == ((DateTime)createdAt).Date) && (reply == null || (x.Reply != null) == reply), x => x.Problem);
        }

        public Report GetDetailById(string Id)
        {
            return _reportRepository.GetReportsDetailSingle(x => x.ID == Id);
        }

        public IEnumerable<Report> GetReportByFilter(string title, string user, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy)
        {
            IEnumerable<Report> data = _reportRepository.GetReportsDetail(x => x.Title.Contains(title) && x.User.Username.Contains(user) && x.Problem.Name.Contains(problem) && (createdAt == null || x.CreatedAt.Date == ((DateTime)createdAt).Date) && (reply == null || (x.Reply != null) == reply));
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

        public IEnumerable<Report> GetReportOfUserByFilter(string title, string userId, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy)
        {
            IEnumerable<Report> data = _reportRepository.GetReportsDetail(x => x.Title.Contains(title) && x.UserID == userId && x.Problem.Name.Contains(problem) && (createdAt == null || x.CreatedAt.Date == ((DateTime)createdAt).Date) && (reply == null || (x.Reply != null) == reply));
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
    }
}
