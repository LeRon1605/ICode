using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Data.Repository;
using Data.Repository.Interfaces;
using Models.DTO;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ProblemService : IProblemService
    {
        private readonly IProblemRepository _problemRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProblemService(IProblemRepository problemRepository, ITagRepository tagRepository, ISubmissionRepository submissionRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _problemRepository = problemRepository;
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
            _submissionRepository = submissionRepository;
            _mapper = mapper;
        }

        #region CRUD 
        public Problem FindByID(string ID)
        {
            return _problemRepository.FindByID(ID);
        }

        public async Task<bool> Remove(string ID)
        {
            Problem problem = _problemRepository.FindByID(ID);
            if (problem == null)
            {
                return false;
            }
            foreach (Submission submission in _submissionRepository.GetSubmissionsDetail(x => x.SubmissionDetails.First().TestCase.ProblemID == ID))
            {
                _submissionRepository.Remove(submission);
            }
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
            Problem problem = _problemRepository.GetProblemDetail(x => x.ID == ID);
            if (problem == null)
            {
                return false;
            }
            ProblemInputUpdate data = entity as ProblemInputUpdate;
            problem.Name = string.IsNullOrWhiteSpace(data.Name) ? problem.Name : data.Name;
            problem.Description = string.IsNullOrWhiteSpace(data.Description) ? problem.Description : data.Description;
            problem.Status = data.Status ?? problem.Status;
            if (data.Tags != null && data.Tags.Length > 0)
            {
                problem.Tags = data.Tags.Select(x => _tagRepository.FindSingle(tag => tag.ID == x)).ToList();
            }
            problem.UpdatedAt = DateTime.Now;
            _problemRepository.Update(problem);
            await _unitOfWork.CommitAsync();
            return true;
        }
        #endregion
        
        public ProblemDTO GetProblemDetail(string ID)
        {
            return _mapper.Map<Problem, ProblemDTO>(_problemRepository.GetProblemDetail(x => x.ID == ID));
        }

        public async Task<PagingList<ProblemDTO>> GetPageByFilter(int page, int pageSize, string name, string author, string tag, DateTime? date, string sort, string orderBy)
        {
            PagingList<Problem> list = await _problemRepository.GetPageAsync(page, pageSize, x => x.Name.Contains(name) && x.Article.Username.Contains(author) && (tag == "" || x.Tags.Any(x => x.Name.Contains(tag))) && (date == null || ((DateTime)date).Date == x.CreatedAt.Date), problem => problem.Article, problem => problem.Tags);
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort)
                {
                    case "name":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.Name) : list.Data.OrderByDescending(x => x.Name);
                        break;
                    case "author":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.Article.Username) : list.Data.OrderByDescending(x => x.Article.Username);
                        break;
                    case "date":
                        list.Data = (orderBy == "asc") ? list.Data.OrderBy(x => x.CreatedAt) : list.Data.OrderByDescending(x => x.CreatedAt);
                        break;
                    default:
                        throw new Exception("Invalid Action.");
                }
            }
            return _mapper.Map<PagingList<Problem>, PagingList<ProblemDTO>>(list);
        }

        public IEnumerable<ProblemDTO> GetProblemsByFilter(string name, string author, string tag, DateTime? date, string sort, string orderBy)
        {
            IEnumerable<ProblemDTO> problems = _mapper.Map<IEnumerable<Problem>, IEnumerable<ProblemDTO>>(_problemRepository.GetProblemDetailMulti(x => x.Name.Contains(name) && x.Article.Username.Contains(author) && (tag == "" || x.Tags.Any(x => x.Name.Contains(tag))) && (date == null || ((DateTime)date).Date == x.CreatedAt.Date)));
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort)
                {
                    case "name":
                        return (orderBy == "asc") ? problems.OrderBy(x => x.Name) : problems.OrderByDescending(x => x.Name);
                    case "author":
                        return (orderBy == "asc") ? problems.OrderBy(x => x.Author.Username) : problems.OrderByDescending(x => x.Author.Username);
                    case "date":
                        return (orderBy == "asc") ? problems.OrderBy(x => x.CreatedAt) : problems.OrderByDescending(x => x.CreatedAt);
                    default:
                        throw new Exception("Invalid Action.");
                }
            }
            return problems;
        }
    }
}
