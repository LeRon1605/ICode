using ICode.Models.Comment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICode.Web.Services.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentDetail>> GetCommentsOfProblem(string problemId);
    }
}
