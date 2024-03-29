﻿using Data;
using Data.Entity;
using Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class SubmissionRepository: BaseRepository<Submission>, ISubmissionRepository
    {
        public SubmissionRepository(ICodeDbContext context): base(context)
        {

        }

        public Submission GetSubmissionDetailSingle(Expression<Func<Submission, bool>> expression)
        {
            return _context.Submissions.Include(x => x.User).Include(x => x.SubmissionDetails).ThenInclude(x => x.TestCase).ThenInclude(x => x.Problem).FirstOrDefault(expression);
        }

        public IEnumerable<Submission> GetSubmissionsDetail(Expression<Func<Submission, bool>> expression = null)
        {
            if (expression == null)
                return _context.Submissions.Include(x => x.User).Include(x => x.SubmissionDetails).ThenInclude(x => x.TestCase).ThenInclude(x => x.Problem);
            else
                return _context.Submissions.Include(x => x.User).Include(x => x.SubmissionDetails).ThenInclude(x => x.TestCase).ThenInclude(x => x.Problem).Where(expression);
        }

        public IEnumerable<Submission> GetSubmissionsOfProblem(string problemID, Expression<Func<Submission, bool>> expression = null)
        {
            if (expression == null)
                return _context.Submissions.Include(x => x.SubmissionDetails).ThenInclude(x => x.TestCase).Where(x => x.SubmissionDetails.Any(s => s.TestCase.ProblemID == problemID));
            else
                return _context.Submissions.Include(x => x.SubmissionDetails).ThenInclude(x => x.TestCase).Where(expression).Where(x => x.SubmissionDetails.Any(s => s.TestCase.ProblemID == problemID));

        }
    }
}
