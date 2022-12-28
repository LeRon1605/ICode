using CodeStudy.Models;
using Data.Entity;
using System.Collections.Generic;

namespace Services.Interfaces
{
    public interface ITestcaseService: IService<TestCase>
    {
        IEnumerable<TestcaseDTO> GetTestcaseOfProblem(string problemId);
    }
}
