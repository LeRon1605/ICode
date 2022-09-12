using Data;
using Data.Entity;
using Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class TestcaseRepository: BaseRepository<TestCase>, ITestcaseRepository
    {
        public TestcaseRepository(ICodeDbContext context): base(context)
        {

        }
    }
}
