using API.Models.DTO;
using API.Models.Entity;
using API.Repository;
using CodeStudy.Models;
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
        public TagService(ITagRepository tagRepository, IUnitOfWork unitOfWork)
        {
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
        }

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

        public bool Exist(string name)
        {
            return _tagRepository.isExist(tag => tag.Name == name);
        }


        public async Task<PagingList<Tag>> GetPageAsync(int page, int pageSize, string keyword)
        {
            return await _tagRepository.GetPageAsync(page, pageSize, tag => tag.Name.Contains(keyword));
        }

        public IEnumerable<Problem> GetProblemOfTag(string Id)
        {
            return _tagRepository.GetTagWithProblem(tag => tag.ID == Id).Problems;
        }

        public IEnumerable<Tag> Find(string name)
        {
            return _tagRepository.FindMulti(x => x.Name.Contains(name));
        }
    }
}
