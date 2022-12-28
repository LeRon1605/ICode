using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Data.Repository;
using Data.Repository.Interfaces;
using Services.Interfaces;
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
        private readonly IMapper _mapper;
        public TestcaseService(ITestcaseRepository testcaseRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _testcaseRepository = testcaseRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region CRUD
        public async Task Add(TestCase testcase)
        {
            await _testcaseRepository.AddAsync(testcase);
            await _unitOfWork.CommitAsync();
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
            TestcaseUpdate data = entity as TestcaseUpdate;
            testcase.Input = (string.IsNullOrWhiteSpace(data.Input)) ? testcase.Input : data.Input;
            testcase.Output = (string.IsNullOrWhiteSpace(data.Output)) ? testcase.Output : data.Output;
            testcase.MemoryLimit = data.MemoryLimit ?? testcase.MemoryLimit;
            testcase.TimeLimit = data.TimeLimit ?? testcase.TimeLimit;
            testcase.UpdatedAt = DateTime.Now;
            _testcaseRepository.Update(testcase);
            await _unitOfWork.CommitAsync();
            return true;
        }
        #endregion

        public IEnumerable<TestcaseDTO> GetTestcaseOfProblem(string problemId)
        {
            return _mapper.Map<IEnumerable<TestCase>, IEnumerable<TestcaseDTO>>(_testcaseRepository.FindMulti(x => x.ProblemID == problemId));
        }
    }
}
