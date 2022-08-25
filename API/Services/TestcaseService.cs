using API.Migrations;
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

        public IEnumerable<TestCase> GetTestcaseOfProblem(string problemId)
        {
            return _testcaseRepository.FindMulti(x => x.ProblemID == problemId);
        }

        public async Task<bool> Remove(string ID)
        {
            TestCase testcase = _testcaseRepository.FindByID(ID);
            if (testcase == null)
            {
                return false;
            }
            _testcaseRepository.Remove(testcase);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public TestCase FindByID(string ID)
        {
            return _testcaseRepository.FindByID(ID);
        }

        public IEnumerable<TestCase> GetAll()
        {
            return _testcaseRepository.FindAll();
        }

        public async Task<bool> Update(string ID, object entity)
        {
            TestCase testcase = _testcaseRepository.FindByID(ID);
            if (testcase == null)
            {
                return false;
            }
            TestcaseInput data = entity as TestcaseInput;
            testcase.Input = (string.IsNullOrWhiteSpace(data.Input)) ? testcase.Input : data.Input;
            testcase.Output = (string.IsNullOrWhiteSpace(data.Output)) ? testcase.Output : data.Output;
            testcase.MemoryLimit = data.MemoryLimit;
            testcase.TimeLimit = data.TimeLimit;
            testcase.UpdatedAt = DateTime.Now;
            _testcaseRepository.Update(testcase);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
