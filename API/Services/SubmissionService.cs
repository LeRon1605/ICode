using API.Helper;
using API.Models.DTO;
using API.Repository;
using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ICodeExecutor _codeExecutor;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITestcaseService _testcaseService;
        private readonly IMapper _mapper;
        public SubmissionService(ISubmissionRepository submissionRepository, IUnitOfWork unitOfWork, ITestcaseService testcaseService, ICodeExecutor codeExecutor, IMapper mapper)
        {
            _submissionRepository = submissionRepository;
            _unitOfWork = unitOfWork;
            _testcaseService = testcaseService;
            _codeExecutor = codeExecutor;
            _mapper = mapper;
        }
        #region CRUD
        public async Task Add(Submission entity)
        {
            await _submissionRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public Submission FindByID(string ID)
        {
            return _submissionRepository.FindByID(ID);
        }

        public IEnumerable<Submission> GetAll()
        {
            return _submissionRepository.FindAll();
        }

        public async Task<bool> Remove(string ID)
        {
            Submission submission = _submissionRepository.FindByID(ID);
            if (submission == null)
            {
                return false;
            }
            _submissionRepository.Remove(submission);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public Task<bool> Update(string ID, object entity)
        {
            throw new NotImplementedException();
        }
        #endregion
        public SubmissionDTO GetDetail(string Id)
        {
            return _mapper.Map<Submission, SubmissionDTO>(_submissionRepository.GetSubmissionDetailSingle(x => x.ID == Id));
        }

        public IEnumerable<SubmissionDetailDTO> GetSubmitDetail(string Id)
        {
            return _mapper.Map<IEnumerable<SubmissionDetail>, IEnumerable<SubmissionDetailDTO>>(_submissionRepository.GetSubmissionDetailSingle(x => x.ID == Id).SubmissionDetails);
        }

        public async Task<PagingList<SubmissionDTO>> GetPageByFilter(int page, int pageSize, string user, string problem, string language, bool? status, DateTime? date, string sort, string orderBy)
        {
            PagingList<Submission> list = await _submissionRepository.GetPageAsync(page, pageSize, x => x.User.Username.Contains(user) && x.SubmissionDetails.First().TestCase.Problem.Name.Contains(problem) && x.Language.Contains(language) && (status == null || (bool)status == x.Status) && (date == null || ((DateTime)date).Date == x.CreatedAt.Date), x => x.SubmissionDetails, x => x.User);
            return _mapper.Map<PagingList<Submission>, PagingList<SubmissionDTO>>(list);
        }

        public IEnumerable<SubmissionDTO> GetSubmissionOfUsers(string userId, bool? status)
        {
            return _mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_submissionRepository.FindMulti(x => x.UserID == userId && (status == null || x.Status == (bool)status)));
        }

        public IEnumerable<SubmissionDTO> GetSubmissionsOfProblem(string problemId)
        {
            return _mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_submissionRepository.GetSubmissionsDetail(x => x.SubmissionDetails.Where(detail => detail.TestCase.ProblemID == problemId).Select(detail => detail.SubmitID).Contains(x.ID)));
        }

        public async Task<SubmissionDTO> Submit(Submission submission, string problemID)
        {
            foreach (TestcaseDTO testcase in _testcaseService.GetTestcaseOfProblem(problemID))
            {
                ExecutorResult result = await _codeExecutor.ExecuteCode(submission.Code, submission.Language, testcase.Input);
                SubmissionDetail submitDetail = new SubmissionDetail
                {
                    Memory = Convert.ToSingle(result.memory),
                    Time = Convert.ToSingle(result.cpuTime),
                    Status = false,
                    TestCaseID = testcase.ID,
                };
                if (result.memory == null && result.cpuTime == null)
                {
                    submitDetail.Description = "Compiler Error";
                    submitDetail.Status = false;
                    submission.Status = false;
                }
                else
                {
                    if (result.output == testcase.Output)
                    {
                        if (Convert.ToSingle(result.cpuTime) <= testcase.TimeLimit)
                        {
                            if (Convert.ToSingle(result.memory) <= testcase.MemoryLimit)
                            {
                                submitDetail.Description = "Success";
                                submitDetail.Status = true;
                                submission.Status = true;
                            }
                            else
                            {
                                submitDetail.Description = "Memory Limit";
                            }
                        }
                        else
                        {
                            submitDetail.Description = "Time Limit";
                        }
                    }
                    else
                    {
                        submitDetail.Description = "Wrong Answer";
                    }
                }
                submission.SubmissionDetails.Add(submitDetail);
            }
            await _submissionRepository.AddAsync(submission);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<Submission, SubmissionDTO>(submission);
        }

        public IEnumerable<SubmissionDTO> GetSubmissionByFilter(string user, string problem, string language, bool? status, DateTime? date, string sort, string orderBy)
        {
            IEnumerable<SubmissionDTO> submissions = _mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_submissionRepository.GetSubmissionsDetail(x => x.User.Username.Contains(user) && x.SubmissionDetails.First().TestCase.Problem.Name.Contains(problem) && x.Language.Contains(language) && (status == null || (bool)status == x.Status) && (date == null || ((DateTime)date).Date == x.CreatedAt.Date)));
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort.ToLower())
                {
                    case "user":
                        return (orderBy == "asc") ? submissions.OrderBy(x => x.User.Username) : submissions.OrderByDescending(x => x.User.Username);
                    case "problem":
                        return (orderBy == "asc") ? submissions.OrderBy(x => x.Problem.Name) : submissions.OrderByDescending(x => x.Problem.Name);
                    case "language":
                        return (orderBy == "asc") ? submissions.OrderBy(x => x.Language) : submissions.OrderByDescending(x => x.Language);
                    case "status":
                        return (orderBy == "asc") ? submissions.OrderBy(x => x.Status) : submissions.OrderByDescending(x => x.Status);
                    case "date":
                        return (orderBy == "asc") ? submissions.OrderBy(x => x.CreatedAt) : submissions.OrderByDescending(x => x.CreatedAt);
                    default:
                        throw new Exception("Invalid Action.");
                }
            }
            return submissions;
        }
    }
}
