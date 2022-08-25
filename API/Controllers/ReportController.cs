using API.Extension;
using API.Filter;
using API.Helper;
using API.Models.DTO;
using API.Models.Entity;
using API.Repository;
using API.Services;
using AutoMapper;
using CodeStudy.Models;
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
        private readonly IMapper _mapper;

        public ReportController(IReportService reportService, IProblemService problemService, IMapper mapper)
        {
            _reportService = reportService;
            _problemService = problemService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReports([FromServices] IDistributedCache cache)
        {
            if (User.FindFirst(Constant.ROLE).Value == Constant.ADMIN)
            {
                IEnumerable<ReportDTO> reports = await cache.GetRecordAsync<IEnumerable<ReportDTO>>("reports_admin");
                if (reports == null)
                {
                    reports = _mapper.Map<IEnumerable<Report>, IEnumerable<ReportDTO>>(_reportService.GetAll());
                    await cache.SetRecordAsync("reports_admin", reports);
                }
                return Ok(reports);
            }
            else
            {
                return Ok(_mapper.Map<IEnumerable<Report>, IEnumerable<ReportDTO>>(_reportService.GetReportsOfUser(User.FindFirst("ID").Value)));
            }  
        }

        [HttpGet("{ID}")]
        [Authorize]
        public IActionResult GetByID(string ID)
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
            if (report.UserID == User.FindFirst(Constant.ID).Value || User.FindFirst(Constant.ID).Value == Constant.ADMIN)
            {
                return Ok(_mapper.Map<Report, ReportDTO>(report));
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
                if (await _reportService.Reply(report, input))
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
                if (await _reportService.UpdateReply(report, input))
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
                if (await _reportService.RemoveReply(report))
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
