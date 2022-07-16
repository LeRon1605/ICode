using API.Filter;
using API.Models.DTO;
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
    [Route("reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ReportController(IUnitOfWork unitOfWork, IReportRepository reportRepository)
        {
            _unitOfWork = unitOfWork;
            _reportRepository = reportRepository;
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetReports()
        {
            if (User.FindFirst("Role")?.Value == "Admin")
            {
                return Ok(new
                {
                    status = true,
                    data = _reportRepository.GetReportsDetail().Select(x => new
                    {
                        ID = x.ID,
                        Content = x.Content,
                        Title = x.Title,
                        ProblemID = x.ProblemID,
                        UserID = x.UserID,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                        Reply = x.Reply == null ? null : new { Content = x.Reply.Content, CreatedAt = x.Reply.CreatedAt, UpdatedAt = x.Reply.UpdatedAt }
                    })
                });
            }
            else
            {
                string userID = User.FindFirst("ID")?.Value;
                return Ok(new
                {
                    status = true,
                    data = _reportRepository.GetReportsDetail(x => x.UserID == userID).Select(x => new
                    {
                        ID = x.ID,
                        Content = x.Content,
                        Title = x.Title,
                        ProblemID = x.ProblemID,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                        Reply = x.Reply == null ? null : new { Content = x.Reply.Content, CreatedAt = x.Reply.CreatedAt, UpdatedAt = x.Reply.UpdatedAt }
                    })
                });
            }  
        }
        [HttpGet("{ID}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        public IActionResult GetByID(string ID)
        {
            Report report = _reportRepository.FindSingle(x => x.ID == ID);
            if (report == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Report Not Found"
                });
            }    
            if (report.UserID == User.FindFirst("ID").Value || User.FindFirst("Role").Value == "Admin")
            {
                return Ok(new
                {
                    status = true,
                    data = new 
                    {
                        ID = report.ID,
                        Title = report.Title,
                        Content = report.Content,
                        UserID = report.UserID,
                        ProblemID = report.ProblemID,
                        CreatedAt = report.CreatedAt,
                        UpdatedAt = report.UpdatedAt
                    }
                });
            }
            else
            {
                return Forbid();
            } 
        }
        [HttpPut("{ID}")]
        [Authorize(Roles = "User")]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        [ServiceFilter(typeof(ExceptionHandler))]
        public IActionResult Update(string ID, ReportInput input)
        {
            Report report = _reportRepository.FindSingle(x => x.ID == ID);
            if (report == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Report Not Found"
                });
            }
            if (report.UserID == User.FindFirst("ID").Value)
            {
                report.Title = input.Title;
                report.Content = input.Content;
                report.UpdatedAt = DateTime.Now;
                _reportRepository.Remove(report);
                _unitOfWork.Commit();
                return NoContent();
            }
            else
            {
                return Forbid();
            }
        }
        [HttpDelete("{ID}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        [ServiceFilter(typeof(ExceptionHandler))]
        public IActionResult Delete(string ID)
        {
            Report report = _reportRepository.FindSingle(x => x.ID == ID);
            if (report == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Report Not Found"
                });
            }
            if (report.UserID == User.FindFirst("ID").Value || User.FindFirst("Role").Value == "Admin")
            {
                _reportRepository.Remove(report);
                _unitOfWork.Commit();
                return NoContent();
            }
            else
            {
                return Forbid();
            }
        }
    }
}
