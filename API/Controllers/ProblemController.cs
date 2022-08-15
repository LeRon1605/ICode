using API.Filter;
using API.Models.DTO;
using API.Models.Entity;
using API.Services;
using AutoMapper;
using CodeStudy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("problems")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        private readonly IProblemService _problemService;
        private readonly ITagService _tagSerivce;
        private readonly ITestcaseService _testcaseService;
        private readonly ISubmissionService _submissionService;
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;

        public ProblemController(IProblemService problemService, ITagService tagService, ISubmissionService submissionService, ITestcaseService testcaseService, IReportService reportService, IMapper mapper)
        {
            _problemService = problemService;
            _tagSerivce = tagService;
            _submissionService = submissionService;
            _testcaseService = testcaseService;
            _reportService = reportService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Problem>, IEnumerable<ProblemDTO>>(_problemService.FindAll()));
        }

        [HttpGet("search")]
        [QueryConstraint(Key = "page")]
        [QueryConstraint(Key = "pageSize")]
        public async Task<IActionResult> Find(int page, int pageSize, string tag = "", string keyword = "")
        {
            PagingList<Problem> list = await _problemService.GetPage(page, pageSize, tag, keyword);
            return Ok(_mapper.Map<PagingList<Problem>, PagingList<ProblemDTO>>(list));
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Create(ProblemInput input)
        {
            if (input.Tags.Any(x => _tagSerivce.FindById(x) == null))
            {
                return NotFound();
            }
            await _problemService.Add(input, User.FindFirst("ID").Value);
            return Ok();
        }

        [HttpGet("{ID}")]
        public IActionResult GetByID(string ID)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound();
            }
            problem.Tags = _problemService.GetTagsOfProblem(ID);
            return Ok(_mapper.Map<Problem, ProblemDTO>(problem));
        }

        [HttpDelete("{ID}")]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Delete(string ID)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound();
            }
            if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
            {
                await _problemService.Remove(ID);
                return NoContent();
            }
            else
            {
                return Forbid();
            }    
        }

        [HttpPut("{ID}")]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Update(string ID, ProblemInputUpdate input)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound();
            }
            if (input.Tags.Any(x => _tagSerivce.FindById(x) == null))
            {
                return NotFound();
            }
            if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
            {
                await _problemService.Update(ID, input);
                return NoContent();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet("{ID}/testcases")]
        [Authorize]
        public IActionResult GetTestCases(string ID)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound();
            }
            if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
            {

                return Ok(_mapper.Map<IEnumerable<TestCase>, IEnumerable<TestcaseDTO>>(_testcaseService.GetTestcaseOfProblem(ID)));
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost("{ID}/testcases")]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> CreateTestCase(string ID, TestcaseInput input)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound();
            }
            if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
            {
                await _testcaseService.Add(new TestCase
                {
                    ID = Guid.NewGuid().ToString(),
                    Input = input.Input,
                    Output = input.Output,
                    CreatedAt = DateTime.Now,
                    MemoryLimit = input.MemoryLimit,
                    TimeLimit = input.TimeLimit,
                    ProblemID = ID
                });
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost("{ID}/submissions")]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Submit(string ID, SubmissionInput input)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound();
            }
            Submission submission = await _submissionService.Submit(new Submission
            {
                ID = Guid.NewGuid().ToString(),
                Status = false,
                UserID = User.FindFirst("ID")?.Value,
                Code = input.Code,
                Language = input.Language,
                CreatedAt = DateTime.Now,
                SubmissionDetails = new List<SubmissionDetail>()
            }, ID);
            
            return Ok(_mapper.Map<Submission, SubmissionDTO>(submission));
        }

        [HttpGet("{ID}/submissions")]
        [Authorize]
        public IActionResult GetSubmitOfProblem(string ID)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_submissionService.GetSubmissionsOfProblem(ID)));
        }

        [HttpPost("{ID}/reports")]
        [Authorize(Roles = "User")]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Report(string ID, ReportInput input)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound();
            }
            await _reportService.Add(new Report
            {
                ID = Guid.NewGuid().ToString(),
                Content = input.Content,
                Title = input.Title,
                ProblemID = ID,
                UserID = User.FindFirst("ID")?.Value,
                CreatedAt = DateTime.Now,
            });
            return Ok();
        }
    }
}
