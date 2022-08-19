using API.Models.Entity;
using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ITestcaseService: IService<TestCase>
    {
        IEnumerable<TestCase> GetTestcaseOfProblem(string problemId);
    }
}
