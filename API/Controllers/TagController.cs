using API.Filter;
using API.Models.DTO;
using API.Models.Entity;
using API.Services;
using AutoMapper;
using CodeStudy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
                return Conflict(new ErrorResponse 
                { 
                    error = "Create tag failed",
                    detail = $"Tag '{input.Name}' already exist."
                });
            }
            else
            {
                await _tagService.Add(input.Name);
                return Ok();
            }
        }   

        [HttpGet("{ID}")]
        [Authorize]
        public IActionResult GetByID(string ID)
        {
            Tag tag = _tagService.FindById(ID);
            if (tag == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Tag does not exist."
                });
            }
            return Ok(_mapper.Map<Tag, TagDTO>(tag));
        }    

        [HttpPut("{ID}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Update(string ID, TagInput input)
        {
            Tag tag = _tagService.FindById(ID);
            if (tag == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Tag does not exist."
                });
            }
            if (await _tagService.Update(tag, input.Name))
            {
                return NoContent();
            }
            else
            {
                return Conflict(new ErrorResponse
                {
                    error = "Create tag failed",
                    detail = $"Tag '{input.Name}' already exist."
                });
            }
        }

        [HttpDelete("{ID}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Delete(string ID)
        {
            Tag tag = _tagService.FindById(ID);
            if (tag == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Tag does not exist."
                });
            }
            await _tagService.Remove(tag);
            return NoContent();
        }

        [HttpGet("{ID}/problems")]
        public IActionResult GetProblemOfTag(string ID)
        {
            Tag tag = _tagService.FindById(ID);
            if (tag == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Tag does not exist."
                });
            }
            return Ok(new
            {
                tag = _mapper.Map<Tag, TagDTO>(tag),
                problems = _mapper.Map<IEnumerable<Problem>, IEnumerable<ProblemDTO>>(_tagService.GetProblemOfTag(ID))
            });
        }
    }
}
