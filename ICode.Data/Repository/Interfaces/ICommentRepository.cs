using Data.Repository.Interfaces;
using ICode.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ICode.Data.Repository.Interfaces
{
    public interface ICommentRepository: IRepository<Comment>
    {
        IEnumerable<Comment> GetCommentsOfProblem(string problemId);
    }
}
