using API.Models.Entity;
using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ITestcaseService
    {
        Task Add(TestCase testcase);
        Task<bool> Update(string ID, TestcaseInput input);
        Task<bool> Remove(string ID);
        TestCase FindById(string Id);
        IEnumerable<TestCase> GetTestcaseOfProblem(string problemId);
    }
}
