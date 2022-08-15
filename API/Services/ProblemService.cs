using API.Models.DTO;
using API.Models.Entity;
using API.Repository;
using CodeStudy.Models;
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

        public async Task Add(ProblemInput input, string authorID)
        {
            await _problemRepository.AddAsync(new Problem
            {
                ID = Guid.NewGuid().ToString(),
                Name = input.Name,
                Status = false,
                Description = input.Description,
                ArticleID = authorID,
                CreatedAt = DateTime.Now,
                TestCases = input.TestCases.Select(x => new TestCase
                {
                    ID = Guid.NewGuid().ToString(),
                    Input = x.Input,
                    Output = x.Output,
                    CreatedAt = DateTime.Now,
                    MemoryLimit = x.MemoryLimit,
                    TimeLimit = x.TimeLimit,
                }).ToList(),
                Tags = input.Tags.Select(x => _tagRepository.FindSingle(tag => tag.ID == x)).ToList()
            });
            await _unitOfWork.CommitAsync();
        }
        public ICollection<Tag> GetTagsOfProblem(string ID)
        {
            return _problemRepository.GetProblemDetail(x => x.ID == ID).Tags;
        }

        public IEnumerable<Problem> FindAll()
        {
            return _problemRepository.GetProblemDetailMulti();
        }

        public Problem FindByID(string ID)
        {
            return _problemRepository.FindSingle(x => x.ID == ID);
        }

        public async Task<PagingList<Problem>> GetPage(int page, int pageSize, string tag, string keyword)
        {
            return await _problemRepository.GetPageAsync(page, pageSize, problem => (keyword == "" || problem.Tags.Any(x => x.Name.Contains(tag)) && problem.Name.Contains(keyword)), problem => problem.Tags, problem => problem.Article); 
        }

        public async Task<bool> Remove(string ID)
        {
            Problem problem = _problemRepository.FindSingle(x => x.ID == ID);
            if (problem == null) return false;
            _problemRepository.Remove(problem);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> Update(string ID, ProblemInputUpdate input)
        {
            Problem problem = _problemRepository.FindSingle(x => x.ID == ID);
            if (problem == null) return false;
            problem.Name = input.Name;
            problem.Description = input.Description;
            problem.Status = input.Status;
            problem.Tags = input.Tags.Select(x => _tagRepository.FindSingle(tag => tag.ID == x)).ToList();
            problem.UpdatedAt = DateTime.Now;
            _problemRepository.Update(problem);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
