using API.Models.DTO;
using API.Repository;
using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace API.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TagService(ITagRepository tagRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #region CRUD
        public IEnumerable<Tag> GetAll()
        {
            return _tagRepository.FindAll();
        }

        public async Task Add(Tag entity)
        {
            await _tagRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public Tag FindByID(string ID)
        {
            return _tagRepository.FindByID(ID);
        }

        public async Task<bool> Remove(string ID)
        {
            Tag tag = _tagRepository.FindByID(ID);
            if (tag == null)
            {
                return false;
            }
            _tagRepository.Remove(tag);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> Update(string ID, object entity)
        {
            TagInput data = entity as TagInput;
            Tag tag = _tagRepository.FindByID(ID);
            if (tag == null || _tagRepository.isExist(tag => tag.Name == data.Name))
            {
                return false;
            }
            tag.Name = (string.IsNullOrWhiteSpace(data.Name)) ? tag.Name : data.Name;
            tag.UpdatedAt = DateTime.Now;
            _tagRepository.Update(tag);
            await _unitOfWork.CommitAsync();
            return true;
        }
        #endregion

        public bool Exist(string name)
        {
            return _tagRepository.isExist(tag => tag.Name == name);
        }


        public async Task<PagingList<TagDTO>> GetPageByFilter(int page, int pageSize, string name, DateTime? date, string sort, string orderBy)
        {
            PagingList<Tag> list = await _tagRepository.GetPageAsync(page, pageSize, tag => tag.Name.Contains(name) && (date == null || tag.CreatedAt.Date == ((DateTime)date).Date));
            return _mapper.Map<PagingList<Tag>, PagingList<TagDTO>>(list);
        }

        public IEnumerable<ProblemDTO> GetProblemOfTag(string Id, string name, DateTime? date, string sort, string orderBy)
        {
            IEnumerable<ProblemDTO> problem = _mapper.Map<IEnumerable<Problem>, IEnumerable<ProblemDTO>>(_tagRepository.GetTagWithProblem(tag => tag.ID == Id).Problems.Where(x => x.Name.Contains(name) && (date == null || ((DateTime)date).Date == x.CreatedAt.Date)));
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort)
                {
                    case "name":
                        return (orderBy == "asc") ? problem.OrderBy(x => x.Name) : problem.OrderByDescending(x => x.Name);
                    case "date":
                        return (orderBy == "asc") ? problem.OrderBy(x => x.CreatedAt) : problem.OrderByDescending(x => x.CreatedAt);
                }
            }
            return problem;
        }
        public IEnumerable<TagDTO> GetTagsByFilter(string name, DateTime? date, string sort, string orderBy)
        {
            IEnumerable<TagDTO> tags = _mapper.Map<IEnumerable<Tag>, IEnumerable<TagDTO>>(_tagRepository.FindMulti(x => x.Name.Contains(name) && (date == null || x.CreatedAt.Date == ((DateTime)date).Date)));
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort.ToLower())
                {
                    case "name":
                        return (orderBy == "asc") ? tags.OrderBy(x => x.Name) : tags.OrderByDescending(x => x.Name);
                    case "date":
                        return (orderBy == "asc") ? tags.OrderBy(x => x.CreatedAt) : tags.OrderByDescending(x => x.CreatedAt);
                    default:
                        throw new Exception("Invalid Action.");
                }
            }
            return tags;
        }
    }
}
