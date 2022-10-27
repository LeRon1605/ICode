using API.Filter;
using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Models.DTO;
using Services.Interfaces;
using System;
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

        [HttpGet]
        [QueryConstraint(Key = "sort", Value = "name, date", Retrict = false)]
        [QueryConstraint(Key = "orderBy", Value = "asc, desc", Depend = "sort")]
        public async Task<IActionResult> GetAll(int? page = null, int pageSize = 5, string name = "", DateTime? date = null, string sort = "", string orderBy = "")
        {
            if (page == null)
            {
                return Ok(_tagService.GetTagsByFilter(name, date, sort, orderBy));
            }
            else
            {
                return Ok(await _tagService.GetPageByFilter((int)page, pageSize, name, date, sort, orderBy));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
                await _tagService.Add(new Tag
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = input.Name,
                    CreatedAt = DateTime.Now
                });
                return Ok();
            }
        }   

        [HttpGet("{ID}")]
        [Authorize]
        public IActionResult GetByID(string ID)
        {
            Tag tag = _tagService.FindByID(ID);
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
        public async Task<IActionResult> Update(string ID, TagInput input)
        {
            Tag tag = _tagService.FindByID(ID);
            if (tag == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Tag does not exist."
                });
            }
            if (await _tagService.Update(ID, input))
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
        public async Task<IActionResult> Delete(string ID)
        {
            if (!await _tagService.Remove(ID))
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Tag does not exist."
                });
            }
            return NoContent();
        }

        [HttpGet("{ID}/problems")]
        public IActionResult GetProblemOfTag(string ID, [FromServices] IDistributedCache cache, string problem = "", DateTime? date = null, string sort = "", string orderBy = "")
        {
            Tag tag = _tagService.FindByID(ID);
            if (tag == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Tag does not exist."
                });
            }
            return Ok(_tagService.GetProblemOfTag(ID, problem, date, sort, orderBy));
        }
    }
}
