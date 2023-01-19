using ICode.Data.Entity;
using Services.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace ICode.API.Services.Interfaces
{
    public interface ICommentService: IService<Comment>
    {
        IEnumerable<Comment> GetCommentsOfProblem(string problemId);
    }
}
