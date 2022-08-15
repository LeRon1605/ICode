using API.Models.Entity;
using API.Repository;
using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class TestcaseService : ITestcaseService
    {
        private readonly ITestcaseRepository _testcaseRepository;
        private readonly IUnitOfWork _unitOfWork;
        public TestcaseService(ITestcaseRepository testcaseRepository, IUnitOfWork unitOfWork)
        {
            _testcaseRepository = testcaseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Add(TestCase testcase)
        {
            await _testcaseRepository.AddAsync(testcase);
            await _unitOfWork.CommitAsync();
        }

        public TestCase FindById(string Id)
        {
            return _testcaseRepository.FindSingle(x => x.ID == Id);
        }

        public IEnumerable<TestCase> GetTestcaseOfProblem(string problemId)
        {
            return _testcaseRepository.FindMulti(x => x.ProblemID == problemId);
        }

        public async Task<bool> Remove(string ID)
        {
            TestCase testcase = _testcaseRepository.FindSingle(x => x.ID == ID);
            if (testcase == null) return false;
            _testcaseRepository.Remove(testcase);
            await _unitOfWork.CommitAsync();
            return true;
        }
        public async Task<bool> Update(string ID, TestcaseInput input)
        {
            TestCase testcase = _testcaseRepository.FindSingle(x => x.ID == ID);
            if (testcase == null) return false;
            testcase.Input = input.Input;
            testcase.Output = input.Output;
            testcase.MemoryLimit = input.MemoryLimit;
            testcase.TimeLimit = input.TimeLimit;
            _testcaseRepository.Update(testcase);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
