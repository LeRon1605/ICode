using API.Models.DTO;
using API.Models.Entity;
using API.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;
        public TagService(ITagRepository tagRepository, IUnitOfWork unitOfWork)
        {
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Add(string name)
        {
            Tag tag = new Tag
            {
                ID = Guid.NewGuid().ToString(),
                Name = name,
                CreatedAt = DateTime.Now
            };
            await _tagRepository.AddAsync(tag);
            await _unitOfWork.CommitAsync();
        }

        public bool Exist(string name)
        {
            return _tagRepository.isExist(tag => tag.Name == name);
        }

        public Tag FindById(string Id)
        {
            return _tagRepository.FindSingle(x => x.ID == Id);
        }

        public IEnumerable<Tag> GetAll()
        {
            return _tagRepository.FindAll();
        }

        public async Task<PagingList<Tag>> GetPageAsync(int page, int pageSize, string keyword)
        {
            return await _tagRepository.GetPageAsync(page, pageSize, tag => tag.Name.Contains(keyword));
        }

        public IEnumerable<Problem> GetProblemOfTag(string Id)
        {
            return _tagRepository.GetTagWithProblem(tag => tag.ID == Id).Problems;
        }

        public async Task Remove(Tag tag)
        {
            _tagRepository.Remove(tag);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> Update(Tag tag, string name)
        {
            if (_tagRepository.isExist(tag => tag.Name == name))
            {
                return false;
            }
            else
            {
                tag.Name = name;
                tag.UpdatedAt = DateTime.Now;
                _tagRepository.Update(tag);
                await _unitOfWork.CommitAsync();
                return true;
            }
        }
    }
}
