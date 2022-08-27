using API.Migrations;
using API.Models.DTO;
using API.Repository;
using CodeStudy.Models;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class ProblemService : IProblemService
    {
        private readonly IProblemRepository _problemRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProblemService(IProblemRepository problemRepository, ITagRepository tagRepository, IUnitOfWork unitOfWork)
        {
            _problemRepository = problemRepository;
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
        }

        public ICollection<Tag> GetTagsOfProblem(string ID)
        {
            return _problemRepository.GetProblemDetail(x => x.ID == ID).Tags;
        }


        public Problem FindByID(string ID)
        {
            return _problemRepository.FindByID(ID);
        }

        public async Task<PagingList<Problem>> GetPage(int page, int pageSize, string tag, string keyword)
        {
            return await _problemRepository.GetPageAsync(page, pageSize, problem => (problem.Tags.Any(x => x.Name.Contains(tag)) && problem.Name.Contains(keyword)), problem => problem.Tags, problem => problem.Article); 
        }

        public async Task<bool> Remove(string ID)
        {
            Problem problem = _problemRepository.FindByID(ID);
            if (problem == null) return false;
            _problemRepository.Remove(problem);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public IEnumerable<Problem> GetAll()
        {
            return _problemRepository.GetProblemDetailMulti();
        }

        public async Task Add(Problem entity)
        {
            await _problemRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> Update(string ID, object entity)
        {
            Problem problem = _problemRepository.FindByID(ID);
            if (problem == null) return false;
            ProblemInputUpdate data = entity as ProblemInputUpdate;
            problem.Name = data.Name;
            problem.Description = data.Description;
            problem.Status = data.Status;
            problem.Tags = data.Tags.Select(x => _tagRepository.FindSingle(tag => tag.ID == x)).ToList();
            problem.UpdatedAt = DateTime.Now;
            _problemRepository.Update(problem);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
