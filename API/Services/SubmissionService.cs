using API.Helper;
using API.Models.DTO;
using API.Models.Entity;
using API.Repository;
using CodeStudy.Models;
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
        public SubmissionService(ISubmissionRepository submissionRepository, IUnitOfWork unitOfWork, ITestcaseService testcaseService, ICodeExecutor codeExecutor)
        {
            _submissionRepository = submissionRepository;
            _unitOfWork = unitOfWork;
            _testcaseService = testcaseService;
            _codeExecutor = codeExecutor;
        }

        public IEnumerable<SubmissionDetail> GetDetail(string Id)
        {
            return _submissionRepository.GetSubmissionDetailSingle(x => x.ID == Id).SubmissionDetails;
        }

        public async Task<PagingList<Submission>> GetPageAsync(int page, int pageSize, bool? status, string keyword)
        {
            return await _submissionRepository.GetPageAsync(page, pageSize, submission => (keyword == "" || submission.User.Username.Contains(keyword)) && (status == null || submission.Status == status), submission => submission.SubmissionDetails);
        }

        public IEnumerable<Submission> GetSubmissionOfUsers(string userId, bool? status)
        {
            return _submissionRepository.FindMulti(x => x.UserID == userId && (status == null || x.Status == (bool)status));
        }

        public IEnumerable<Submission> GetSubmissionsOfProblem(string problemId)
        {
            return _submissionRepository.GetSubmissionsDetail(x => x.SubmissionDetails.Where(detail => detail.TestCase.ProblemID == problemId).Select(detail => detail.SubmitID).Contains(x.ID));
        }

        public async Task<Submission> Submit(Submission submission, string problemID)
        {
            foreach (TestCase testcase in _testcaseService.GetTestcaseOfProblem(problemID))
            {
                ExecutorResult result = await _codeExecutor.ExecuteCode(submission.Code, submission.Language, testcase.Input);
                SubmissionDetail submitDetail = new SubmissionDetail
                {
                    Memory = Convert.ToSingle(result.memory),
                    Time = Convert.ToSingle(result.cpuTime),
                    Status = false,
                    TestCaseID = testcase.ID,
                };
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
                submission.SubmissionDetails.Add(submitDetail);
            }
            await _submissionRepository.AddAsync(submission);
            await _unitOfWork.CommitAsync();
            return submission;
        }

        public async Task Add(Submission entity)
        {
            await _submissionRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public Submission FindByID(string ID)
        {
            return _submissionRepository.FindSingle(submission => submission.ID == ID);
        }

        public IEnumerable<Submission> GetAll()
        {
            return _submissionRepository.FindAll();
        }

        public async Task<bool> Remove(string ID)
        {
            Submission submission = _submissionRepository.FindSingle(submission => submission.ID == ID);
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
    }
}
