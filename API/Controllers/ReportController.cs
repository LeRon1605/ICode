using API.Extension;
using API.Filter;
using API.Helper;
using API.Models.DTO;
using API.Repository;
using API.Services;
using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IProblemService _problemService;
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService, IProblemService problemService)
        {
            _reportService = reportService;
            _problemService = problemService;
        }

        [HttpGet]
        [Authorize]
        [QueryConstraint(Key = "sort", Value = "user, problem, title, date, reply", Retrict = false)]
        [QueryConstraint(Key = "orderBy", Value = "asc, desc", Depend = "sort")]

        public async Task<IActionResult> GetReports(int? page = null, int pageSize = 5, string title = "", string user = "", string problem = "", DateTime? date = null, bool? reply = null, string sort = "", string orderBy = "")
        {
            if (User.FindFirst(Constant.ROLE).Value == Constant.ADMIN)
            {
                // Admin get all reports
                if (page == null)
                {
                    return Ok(_reportService.GetReportByFilter(title, user, problem, date, reply, sort, orderBy));
                }
                else
                {
                    return Ok(await _reportService.GetPageAsync((int)page, pageSize, title, user, problem, date, reply, sort, orderBy));
                }
            }
            else
            {
                // User get only their reports
                if (page == null)
                {
                    return Ok(_reportService.GetReportOfUserByFilter(title, User.FindFirst(Constant.ID).Value, problem, date, reply, sort, orderBy));
                }
                else
                {
                    return Ok(await _reportService.GetPageReportOfUser((int)page, pageSize, title, User.FindFirst(Constant.ID).Value, problem, date, reply, sort, orderBy));
                }
            }  
        }

        [HttpGet("{ID}")]
        [Authorize]
        public IActionResult GetByID(string ID)
        {
            ReportDTO report = _reportService.GetDetailById(ID);
            if (report == null)
            {
                return NotFound(new ErrorResponse 
                { 
                    error = "Resource not found.",
                    detail = "Report does not exist."
                });
            }    
            if (report.User.ID == User.FindFirst(Constant.ID).Value || User.FindFirst(Constant.ID).Value == Constant.ADMIN)
            {
                return Ok(report);
            }
            else
            {
                return Forbid();
            } 
        }

        [HttpPut("{ID}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Update(string ID, ReportInput input)
        {
            Report report = _reportService.FindByID(ID);
            if (report == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Report does not exist."
                });
            }
            if (report.UserID == User.FindFirst(Constant.ID).Value)
            {
                await _reportService.Update(ID, input);
                return NoContent();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpDelete("{ID}")]
        [Authorize]
        public async Task<IActionResult> Delete(string ID)
        {
            Report report = _reportService.FindByID(ID);
            if (report == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Report does not exist."
                });
            }
            if (report.UserID == User.FindFirst(Constant.ID).Value || User.FindFirst(Constant.ROLE).Value == Constant.ADMIN)
            {
                await _reportService.Remove(ID);
                return NoContent();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost("{ID}/reply")]
        [Authorize]
        public async Task<IActionResult> Reply(string ID, ReplyInput input)
        {
            Report report = _reportService.FindByID(ID);
            if (report == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Report does not exist."
                });
            }
            if (_problemService.FindByID(report.ProblemID).ArticleID == User.FindFirst(Constant.ID).Value || User.FindFirst(Constant.ROLE).Value == Constant.ADMIN)
            {
                if (await _reportService.Reply(ID, input))
                {
                    return NoContent();
                }
                else
                {
                    return Conflict(new ErrorResponse
                    {
                        error = "Reply failed.",
                        detail = "This report has already replied."
                    });
                }              
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPut("{ID}/reply")]
        [Authorize]
        public async Task<IActionResult> UpdateReply(string ID, ReplyInput input)
        {
            Report report = _reportService.FindByID(ID);
            if (report == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Report does not exist."
                });
            }
            if (_problemService.FindByID(report.ProblemID).ArticleID == User.FindFirst(Constant.ID).Value || User.FindFirst(Constant.ROLE).Value == Constant.ADMIN)
            {
                if (await _reportService.UpdateReply(ID, input))
                {
                    return NoContent();
                }
                else
                {
                    return NotFound(new ErrorResponse
                    {
                        error = "Resource not found.",
                        detail = "This report has not been replied yet."
                    });
                }
            }
            else
            {
                return Forbid();
            }
        }

        [HttpDelete("{ID}/reply")]
        [Authorize]
        public async Task<IActionResult> DeleteReply(string ID)
        {
            Report report = _reportService.FindByID(ID);
            if (report == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Report does not exist."
                });
            }
            if (_problemService.FindByID(report.ProblemID).ArticleID == User.FindFirst(Constant.ID).Value || User.FindFirst(Constant.ROLE).Value == Constant.ADMIN)
            {
                if (await _reportService.RemoveReply(ID))
                {
                    return NoContent();
                }
                else
                {
                    return NotFound(new ErrorResponse
                    {
                        error = "Resource not found.",
                        detail = "This report has not been replied yet."
                    });
                }
            }
            else
            {
                return Forbid();
            }
        }
    }
}
