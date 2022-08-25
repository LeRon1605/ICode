using API.Extension;
using API.Filter;
using API.Models.DTO;
using API.Models.Entity;
using API.Services;
using AutoMapper;
using CodeStudy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static StackExchange.Redis.Role;

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
        public async Task<IActionResult> GetAll([FromServices] IDistributedCache cache, int? page = null, int pageSize = 5, string name = "")
        {
            if (page == null)
            {
                CacheData data = await cache.GetRecordAsync<CacheData>("tags");
                if (data == null)
                {
                    data = new CacheData
                    {
                        RecordID = "tags",
                        Data = _mapper.Map<IEnumerable<Tag>, IEnumerable<TagDTO>>(_tagService.GetAll()),
                        CacheAt = DateTime.Now,
                        ExpireAt = DateTime.Now.AddSeconds(60)
                    };
                    await cache.SetRecordAsync("tags", data);
                }
                return Ok(data);
            }
            else
            {
                PagingList<Tag> list = await _tagService.GetPageAsync((int)page, pageSize, name);
                return Ok(_mapper.Map<PagingList<Tag>, PagingList<TagDTO>>(list));
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
        public async Task<IActionResult> GetProblemOfTag(string ID, [FromServices] IDistributedCache cache)
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
            IEnumerable<ProblemDTO> problems = await cache.GetRecordAsync<IEnumerable<ProblemDTO>>($"tag_problems_{ID}");
            if (problems == null)
            {
                problems = _mapper.Map<IEnumerable<Problem>, IEnumerable<ProblemDTO>>(_tagService.GetProblemOfTag(ID));
                await cache.SetRecordAsync($"tag_problems_{ID}", problems);
            }
            return Ok(new
            {
                tag = _mapper.Map<Tag, TagDTO>(tag),
                problems = problems
            });
        }
    }
}
