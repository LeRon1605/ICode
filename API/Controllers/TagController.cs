using API.Filter;
using API.Models.DTO;
using API.Models.Entity;
using API.Repository;
using AutoMapper;
using CodeStudy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("tags")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TagController(IUnitOfWork unitOfWork, ITagRepository tagRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }
        [HttpGet("search")]
        public async Task<IActionResult> Find(int page, int pageSize, string keyword = "")
        {
            PagingList<Tag> list = await _tagRepository.GetPageAsync(page, pageSize, tag => tag.Name.Contains(keyword));
            return Ok(_mapper.Map<PagingList<Tag>, PagingList<TagDTO>>(list));
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Tag>, IEnumerable<TagDTO>>(_tagRepository.FindAll()));
        }
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Create(TagInput input)
        {
            if (_tagRepository.isExist(tag => tag.Name == input.Name))
            {
                return Conflict(new
                {
                    status = false,
                    message = $"Tag với tên '{input.Name}' đã tồn tại"
                });
            }
            else
            {
                Tag tag = new Tag
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = input.Name,
                    CreatedAt = DateTime.Now
                };
                await _tagRepository.AddAsync(tag);
                await _unitOfWork.CommitAsync();
                return CreatedAtAction("GetByID", "Tag", new { id = tag.ID }, new { status = true, message = "Tạo mới tag thành công" });
            }
        }   
        [HttpGet("{ID}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        public IActionResult GetByID(string ID)
        {
            Tag tag = _tagRepository.FindSingle(x => x.ID == ID);
            if (tag == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Tag not found"
                });
            }
            return Ok(new
            {
                status = true,
                data = _mapper.Map<Tag, TagDTO>(tag)
            });
        }    
        [HttpPut("{ID}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(ExceptionHandler))]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        public async Task<IActionResult> Update(string ID, TagInput input)
        {
            Tag tag = _tagRepository.FindSingle(x => x.ID == ID);
            if (tag == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Tag not found"
                });
            }    
            if (_tagRepository.isExist(tag => tag.Name == input.Name))
            {
                return Conflict(new
                {
                    status = false,
                    message = $"Tag với tên '{input.Name}' đã tồn tại"
                });
            }
            else
            {
                tag.Name = input.Name;
                tag.UpdatedAt = DateTime.Now;
                _tagRepository.Update(tag);
                await _unitOfWork.CommitAsync();
                return NoContent();
            }
        }
        [HttpDelete("{ID}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(ExceptionHandler))]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        public async Task<IActionResult> Delete(string ID)
        {
            Tag tag = _tagRepository.FindSingle(x => x.ID == ID);
            if (tag == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Tag not found"
                });
            }
            _tagRepository.Remove(tag);
            await _unitOfWork.CommitAsync();
            return NoContent();
        }
        [HttpGet("{ID}/problems")]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        public IActionResult GetProblemOfTag(string ID)
        {
            Tag tag = _tagRepository.GetTagWithProblem(x => x.ID == ID);
            if (tag == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Tag not found"
                });
            }
            return Ok(new
            {
                status = true,
                data = new
                {
                    tagID = tag.ID, 
                    Name = tag.Name,
                    problems = tag.Problems.Select(x => new 
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Description = x.Description,
                        ArticleID = x.ArticleID,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                }
            });
        }
    }
}
