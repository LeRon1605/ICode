using API.Filter;
using API.Helper;
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
        public IActionResult GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_submissionService.FindAll()));
        }

        [HttpGet("search")]
        [QueryConstraint(Key = "page")]
        [QueryConstraint(Key = "pageSize")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Find(int page, int pageSize, bool? status = null, string keyword = "")
        {
            PagingList<Submission> list = await _submissionService.GetPageAsync(page, pageSize, status, keyword);
            return Ok(_mapper.Map<PagingList<Submission>, PagingList<SubmissionDTO>>(list));
        }

        [HttpGet("{ID}")]
        [Authorize]
        public IActionResult GetByID(string ID)
        {
            Submission submission = _submissionService.FindById(ID);
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
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Delete(string ID)
        {
            Submission submission = _submissionService.FindById(ID);
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
                await _submissionService.Remove(submission);
                return Ok();
            }
        }
    }
}
