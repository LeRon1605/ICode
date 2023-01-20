using Data;
using Data.Entity;
using Data.Repository;
using ICode.Data.Entity;
using ICode.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace ICode.Data.Repository
{
    public class CommentRepository: BaseRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ICodeDbContext context): base(context)
        {

        }

        public IEnumerable<Comment> GetCommentsOfProblem(string problemId)
        {
            string sql = @$"
                WITH CTE_COMMENTS AS (
	                SELECT * FROM COMMENTS
	                WHERE ParentID IS NULL
	                UNION ALL
	                SELECT COMMENTS.* FROM COMMENTS INNER JOIN CTE_COMMENTS
	                ON CTE_COMMENTS.ID = COMMENTS.ParentID
                )
                SELECT * FROM CTE_COMMENTS 
                WHERE CTE_COMMENTS.PROBLEMID = '{problemId}';
            ";
            IEnumerable<Comment> comments = _context.Comments.FromSqlRaw(sql).ToList().Where(x => x.ParentID == null);
            LoadUserHierachy(comments);
            return comments;
        }

        public void LoadUserHierachy(IEnumerable<Comment> comments)
        {
            foreach (Comment comment in comments)
            {
                _context.Entry(comment).Reference(x => x.User).Load();
                if (comment.Childs != null && comment.Childs.Count > 0)
                {
                    LoadUserHierachy(comment.Childs);
                }
            }
        }

        public void RemoveHierachy(string id)
        {
            string sql = @$"
                WITH CTE_COMMENTS AS (
	                SELECT * FROM COMMENTS
	                WHERE ID = '{id}'
	                UNION ALL
	                SELECT COMMENTS.* FROM COMMENTS INNER JOIN CTE_COMMENTS
	                ON CTE_COMMENTS.ID = COMMENTS.ParentID
                )
                DELETE FROM COMMENTS WHERE ID IN (SELECT ID FROM CTE_COMMENTS) 
            ";
            _context.Database.ExecuteSqlRaw(sql);
        }
    }
}
