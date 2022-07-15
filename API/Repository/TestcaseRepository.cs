using API.Models.Data;
using API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface ITestcaseRepository: IRepository<TestCase>
    {

    }
    public class TestcaseRepository: BaseRepository<TestCase>, ITestcaseRepository
    {
        public TestcaseRepository(ICodeDbContext context): base(context)
        {

        }
    }
}
