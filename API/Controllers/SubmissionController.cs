using API.Filter;
using API.Models.Entity;
using API.Repository;
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
        public SubmissionController(IUnitOfWork unitOfWork, ISubmissionRepository submissionRepository)
        {
            _unitOfWork = unitOfWork;
            _submissionRepository = submissionRepository;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            return Ok(new
            {
                status = true,
                data = _submissionRepository.GetSubmissionsDetail().Select(x => new
                {
                    ID = x.ID,
                    Code = x.Code,
                    Language = x.Language,
                    Status = x.Status,
                    UserID = x.UserID,
                    ProblemID = x.SubmissionDetails.First().TestCase.ProblemID,
                    CreatedAt = x.CreatedAt,
                    details = x.SubmissionDetails.Select(detail => new
                    {
                        TestcaseID = detail.TestCaseID,
                        Time = detail.Time,
                        Memory = detail.Memory,
                        Description = detail.Description,
                        Status = detail.Status
                    })
                })
            });
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
                        status = true,
                        data = new
                        {
                            ID = submission.ID,
                            Code = submission.Code,
                            Language = submission.Language,
                            Status = submission.Status,
                            UserID = submission.UserID,
                            ProblemID = submission.SubmissionDetails.First().TestCase.ProblemID,
                            CreatedAt = submission.CreatedAt,
                            details = submission.SubmissionDetails.Select(detail => new
                            {
                                TestcaseID = detail.TestCaseID,
                                Time = detail.Time,
                                Memory = detail.Memory,
                                Description = detail.Description,
                                Status = detail.Status
                            })
                        }
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
        public IActionResult Delete(string ID)
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
                _unitOfWork.Commit();
                return Ok(new
                {
                    status = true,
                    message = "Delete successfully"
                });
            }
        }
    }
}
