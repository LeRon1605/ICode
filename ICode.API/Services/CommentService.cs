using Data.Repository;
using ICode.API.Services.Interfaces;
using ICode.Data.Entity;
using ICode.Data.Repository.Interfaces;
using ICode.Models.Comment;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICode.API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CommentService(ICommentRepository commentRepository, IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Add(Comment entity)
        {
            await _commentRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public Comment FindByID(string ID)
        {
            return _commentRepository.FindByID(ID);
        }

        public IEnumerable<Comment> GetAll()
        {
            return _commentRepository.FindAll();
        }

        public IEnumerable<Comment> GetCommentsOfProblem(string problemId)
        {
            List<Comment> comments = _commentRepository.GetCommentsOfProblem(problemId).ToList();
            return _commentRepository.GetCommentsOfProblem(problemId).ToList();
        }

        public async Task<bool> Remove(string ID)
        {
            Comment comment = await _commentRepository.FindByIDAsync(ID);
            if (comment != null)
            {
                _commentRepository.Remove(comment);
                await _unitOfWork.CommitAsync();
                return true;
            }    
            return false;
        }

        public async Task<bool> Update(string ID, object entity)
        {
            Comment comment = await _commentRepository.FindByIDAsync(ID);
            if (comment == null)
            {
                return false;
            }    
            CommentUpdateDTO obj = entity as CommentUpdateDTO;
            comment.Content = obj.Content;
            _commentRepository.Update(comment);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
