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
                return NotFound();
            }
            else
            {
                if (User.FindFirst("Role").Value == "Admin" || submission.UserID == User.FindFirst("ID").Value)
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
                return NotFound();
            }
            else
            {
                await _submissionService.Remove(submission);
                return Ok();
            }
        }
    }
}
