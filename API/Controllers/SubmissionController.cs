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
    [Route("submissions")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SubmissionController(IUnitOfWork unitOfWork, ISubmissionRepository submissionRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _submissionRepository = submissionRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_submissionRepository.FindAll()));
        }
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Find(int page, int pageSize, bool? status = null, string keyword = "")
        {
            PagingList<Submission> list = await _submissionRepository.GetPageAsync(page, pageSize, submission => (keyword == "" || submission.User.Username.Contains(keyword)) && (status == null || submission.Status == status), submission => submission.SubmissionDetails);
            return Ok(_mapper.Map<PagingList<Submission>, PagingList<SubmissionDTO>>(list));
        }
        [HttpGet("{ID}")]
        [Authorize]
        public IActionResult GetByID(string ID)
        {
            Submission submission = _submissionRepository.GetSubmissionsDetail(x => x.ID == ID).FirstOrDefault();
            if (submission == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Not found"
                });
            }
            else
            {
                if (User.FindFirst("Role")?.Value == "Admin" || submission.UserID == User.FindFirst("ID")?.Value)
                {
                    return Ok(new
                    {
                        submission = _mapper.Map<Submission, SubmissionDTO>(submission),
                        detail = _mapper.Map<IEnumerable<SubmissionDetail>, IEnumerable<SubmissionDetailDTO>>(submission.SubmissionDetails)
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
            Submission submission = _submissionRepository.FindSingle(x => x.ID == ID);
            if (submission == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Not found"
                });
            }
            else
            {
                _submissionRepository.Remove(submission);
                await _unitOfWork.CommitAsync();
                return Ok(new
                {
                    status = true,
                    message = "Delete successfully"
                });
            }
        }
    }
}
