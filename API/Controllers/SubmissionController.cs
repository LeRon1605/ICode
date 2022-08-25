using API.Extension;
using API.Filter;
using API.Helper;
using API.Models.DTO;
using API.Models.Entity;
using API.Services;
using AutoMapper;
using CodeStudy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("submissions")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;
        private readonly IMapper _mapper;
        public SubmissionController(ISubmissionService submissionService, IMapper mapper)
        {
            _submissionService = submissionService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromServices] IDistributedCache cache)
        {
            IEnumerable<SubmissionDTO> submissions = await cache.GetRecordAsync<IEnumerable<SubmissionDTO>>("submissions");
            bool isFromCache = true;
            if (submissions == null)
            {
                submissions = _mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_submissionService.GetAll());
                await cache.SetRecordAsync("submission", submissions);
                isFromCache = false;
            }
            return Ok(new
            {
                data = submissions,
                from = isFromCache ? "cache" : "db"
            });
        }

        [HttpGet("search")]
        [QueryConstraint(Key = "page")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Find(int page, int pageSize = 5, bool? status = null, string keyword = "")
        {
            PagingList<Submission> list = await _submissionService.GetPageAsync(page, pageSize, status, keyword);
            return Ok(_mapper.Map<PagingList<Submission>, PagingList<SubmissionDTO>>(list));
        }

        [HttpGet("{ID}")]
        [Authorize]
        public IActionResult GetByID(string ID)
        {
            Submission submission = _submissionService.FindByID(ID);
            if (submission == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Submission does not exist."
                });
            }
            else
            {
                if (User.FindFirst(Constant.ROLE).Value == Constant.ADMIN || submission.UserID == User.FindFirst(Constant.ID).Value)
                {
                    return Ok(new
                    {
                        submission = _mapper.Map<Submission, SubmissionDTO>(submission),
                        detail = _mapper.Map<IEnumerable<SubmissionDetail>, IEnumerable<SubmissionDetailDTO>>(_submissionService.GetDetail(ID))
                    });
                }
                else
                {
                    return Forbid();
                }
            }
        }

        [HttpDelete("{ID}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string ID)
        {
            if (!await _submissionService.Remove(ID))
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Submission does not exist."
                });
            }
            else
            {
                return Ok();
            }
        }
    }
}
