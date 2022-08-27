using API.Extension;
using API.Filter;
using API.Helper;
using API.Models.DTO;
using API.Services;
using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using System;
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

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Find([FromServices] IDistributedCache cache, int? page = null, int pageSize = 5, bool? status = null, string user = "")
        {
            if (page == null)
            {
                CacheData data = await cache.GetRecordAsync<CacheData>("submissions");
                if (data == null)
                {
                    data = new CacheData
                    {
                        RecordID = "submissions",
                        CacheAt = DateTime.Now,
                        ExpireAt = DateTime.Now.AddSeconds(60),
                        Data = _mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_submissionService.GetAll())
                    };
                    await cache.SetRecordAsync(data.RecordID, data);
                }
                return Ok(data);
            }
            else
            {
                PagingList<Submission> list = await _submissionService.GetPageAsync(page == null ? 1 : (int)page, pageSize, status, user);
                return Ok(_mapper.Map<PagingList<Submission>, PagingList<SubmissionDTO>>(list));
            }
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
