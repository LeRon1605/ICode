using API.Filter;
using API.Models.DTO;
using API.Models.Entity;
using API.Repository;
using API.Services;
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
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;
        public TagController(ITagService tagService, IMapper mapper)
        {
            _tagService = tagService;
            _mapper = mapper;
        }
        [HttpGet("search")]
        [QueryConstraint(Key = "page")]
        [QueryConstraint(Key = "pageSize")]
        public async Task<IActionResult> Find(int page, int pageSize, string keyword = "")
        {
            PagingList<Tag> list = await _tagService.GetPageAsync(page, pageSize, keyword);
            return Ok(_mapper.Map<PagingList<Tag>, PagingList<TagDTO>>(list));
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Tag>, IEnumerable<TagDTO>>(_tagService.GetAll()));
        }
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Create(TagInput input)
        {
            if (_tagService.Exist(input.Name))
            {
                return Conflict();
            }
            else
            {
                await _tagService.Add(input.Name);
                return Ok();
            }
        }   
        [HttpGet("{ID}")]
        [Authorize]
        [QueryConstraint(Key = "ID")]
        public IActionResult GetByID(string ID)
        {
            Tag tag = _tagService.FindById(ID);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<Tag, TagDTO>(tag));
        }    
        [HttpPut("{ID}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(ExceptionHandler))]
        [QueryConstraint(Key = "ID")]
        public async Task<IActionResult> Update(string ID, TagInput input)
        {
            Tag tag = _tagService.FindById(ID);
            if (tag == null)
            {
                return NotFound();
            }
            if (await _tagService.Update(tag, input.Name))
            {
                return NoContent();
            }
            else
            {
                return Conflict();
            }
        }
        [HttpDelete("{ID}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(ExceptionHandler))]
        [QueryConstraint(Key = "ID")]
        public async Task<IActionResult> Delete(string ID)
        {
            Tag tag = _tagService.FindById(ID);
            if (tag == null)
            {
                return NotFound();
            }
            await _tagService.Remove(tag);
            return NoContent();
        }
        [HttpGet("{ID}/problems")]
        [QueryConstraint(Key = "ID")]
        public IActionResult GetProblemOfTag(string ID)
        {
            Tag tag = _tagService.FindById(ID);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                tag = _mapper.Map<Tag, TagDTO>(tag),
                problems = _mapper.Map<IEnumerable<Problem>, IEnumerable<ProblemDTO>>(_tagService.GetProblemOfTag(ID))
            });
        }
    }
}
