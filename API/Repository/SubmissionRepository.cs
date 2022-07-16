using API.Models.Data;
using API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface ISubmissionRepository: IRepository<Submission>
    {

    }
    public class SubmissionRepository: BaseRepository<Submission>, ISubmissionRepository
    {
        public SubmissionRepository(ICodeDbContext context): base(context)
        {

        }
    }
}
