using API.Filter;
using CodeStudy.Models;
using Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Models.DTO;
using Services.Interfaces;
using ICode.Common;

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
        private readonly ILogger<ProblemController> _logger;

        public ProblemController(IProblemService problemService, ITagService tagService, ISubmissionService submissionService, ITestcaseService testcaseService, IReportService reportService, ILogger<ProblemController> logger)
        {
            _problemService = problemService;
            _tagSerivce = tagService;
            _submissionService = submissionService;
            _testcaseService = testcaseService;
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet]
        [QueryConstraint(Key = "sort", Value = "name, author, date", Retrict = false)]
        [QueryConstraint(Key = "orderBy", Value = "asc, desc", Depend = "sort")]
        public async Task<IActionResult> GetAll(int? page = null, int pageSize = 5, string name = "", string author = "", string tag = "", DateTime? date = null, string sort = "", string orderBy = "", Level? level = null)
        {
            if (page != null)
            {
                return Ok(await _problemService.GetPageByFilter((int)page, pageSize, name, author, tag, date, level, sort, orderBy));
            }
            else
            {
                return Ok(_problemService.GetProblemsByFilter(name, author, tag, date, sort, orderBy));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ProblemInput input)
        {
            string[] tags = input.Tags.Where(x => _tagSerivce.FindByID(x) == null).ToArray();
            if (tags.Count() > 0)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Create problem failed.",
                    detail = new
                    {
                        message = "Tags does not exist.",
                        value = tags
                    }
                });
            }
            Problem problem = new Problem
            {
                ID = Guid.NewGuid().ToString(),
                Name = input.Name,
                Status = false,
                Description = input.Description,
                ArticleID = User.FindFirst(Constant.ID).Value,
                CreatedAt = DateTime.Now,
                Level = input.Level,
                Score = input.Score,
                TestCases = input.TestCases.Select(x => new TestCase
                {
                    ID = Guid.NewGuid().ToString(),
                    Input = x.Input,
                    Output = x.Output,
                    CreatedAt = DateTime.Now,
                    MemoryLimit = x.MemoryLimit,
                    TimeLimit = x.TimeLimit,
                }).ToList(),
                Tags = input.Tags.Select(x => _tagSerivce.FindByID(x)).ToList()
            };
            await _problemService.Add(problem);
            return Ok();
        }

        [HttpGet("{ID}")]
        public IActionResult GetByID(string ID)
        {
            ProblemDTO problem = _problemService.GetProblemDetail(ID);
            if (problem == null)
            {
                return NotFound(new ErrorResponse { 
                    error = "Resource not found.",
                    detail = "Problem does not exist."
                });
            }
            return Ok(problem);
        }

        [HttpDelete("{ID}")]
        [Authorize]
        public async Task<IActionResult> Delete(string ID)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Problem does not exist."
                });
            }
            if (problem.ArticleID == User.FindFirst(Constant.ID)?.Value || User.FindFirst(Constant.ROLE)?.Value == Constant.ADMIN)
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
        public async Task<IActionResult> Update(string ID, ProblemInputUpdate input)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Problem does not exist."
                });
            }
            if (input.Tags != null && input.Tags.Length > 0)
            {
                string[] tags = input.Tags.Where(x => _tagSerivce.FindByID(x) == null).ToArray();
                if (tags.Count() > 0)
                {
                    return NotFound(new ErrorResponse
                    {
                        error = "Create problem failed.",
                        detail = new
                        {
                            message = "Tags does not exist.",
                            value = tags
                        }
                    });
                }
            }
            if (problem.ArticleID == User.FindFirst(Constant.ID)?.Value || User.FindFirst(Constant.ROLE)?.Value == Constant.ADMIN)
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
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Problem does not exist."
                });
            }
            if (problem.ArticleID == User.FindFirst(Constant.ID)?.Value || User.FindFirst(Constant.ROLE)?.Value == Constant.ADMIN)
            {

                return Ok(_testcaseService.GetTestcaseOfProblem(ID));
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost("{ID}/testcases")]
        [Authorize]
        public async Task<IActionResult> CreateTestCase(string ID, TestcaseInput input)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Problem does not exist."
                });
            }
            if (problem.ArticleID == User.FindFirst(Constant.ID).Value || User.FindFirst(Constant.ROLE).Value == Constant.ADMIN)
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
        public async Task<IActionResult> Submit(string ID, SubmissionInput input)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Problem does not exist."
                });
            }
            SubmissionResult submission = await _submissionService.Submit(new Submission
            {
                ID = Guid.NewGuid().ToString(),
                State = SubmitState.Pending,
                UserID = User.FindFirst(Constant.ID).Value,
                Code = input.Code,
                Language = input.Language,
                ProblemID = ID,
                CreatedAt = DateTime.Now,
                SubmissionDetails = new List<SubmissionDetail>()
            });
            
            return Ok(submission);
        }

        [HttpPost("{ID}/submissions")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Submit(string ID, IFormFile file)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Problem does not exist."
                });
            }
            if (file != null && file.Length > 0)
            {
                string code = null;
                using (var stream = new StreamReader(file.OpenReadStream()))
                {
                    code = await stream.ReadToEndAsync();
                };
                if (code == null)
                {
                    _logger.LogWarning("Cant read content from {File} At {Date} while Content Length is {Length}", file.FileName, DateTime.UtcNow, file.Length);
                    return BadRequest(new
                    {
                        error = "Submit failed.",
                        detail = "Invalid file, can't read content from file."
                    });
                }
                SubmissionResult submission = await _submissionService.Submit(new Submission
                {
                    ID = Guid.NewGuid().ToString(),
                    State = SubmitState.Pending,
                    UserID = User.FindFirst(Constant.ID).Value,
                    Code = code,
                    ProblemID = ID,
                    Language = "cpp",
                    CreatedAt = DateTime.Now,
                    SubmissionDetails = new List<SubmissionDetail>()
                });
                return Ok(submission);
            }
            return BadRequest(new ErrorResponse
            {
                error = "Submit failed.",
                detail = "File empty."
            });
        }

        [HttpGet("{ID}/submissions")]
        [QueryConstraint(Key = "sort", Value = "user, language, status, date", Retrict = false)]
        [QueryConstraint(Key = "orderBy", Value = "asc, desc", Depend = "sort")]
        public async Task<IActionResult> GetSubmitOfProblem(string ID, int? page = null, int pageSize = 5, string user = "", string language = "", bool? status = null, DateTime? date = null, string sort = "", string orderBy = "")
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Problem does not exist."
                });
            }
            if (page != null)
            {
                return Ok(await _submissionService.GetPageSubmissionsOfProblem(ID, (int)page, pageSize, user, language, status, date, sort, orderBy));
            }
            else
            {
                return Ok(_submissionService.GetSubmissionsOfProblem(ID));
            }
        }

        [HttpPost("{ID}/reports")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Report(string ID, ReportInput input)
        {
            Problem problem = _problemService.FindByID(ID);
            if (problem == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Problem does not exist."
                });
            }
            await _reportService.Add(new Report
            {
                ID = Guid.NewGuid().ToString(),
                Content = input.Content,
                Title = input.Title,
                ProblemID = ID,
                UserID = User.FindFirst(Constant.ID).Value,
                CreatedAt = DateTime.Now,
            });
            return Ok();
        }

        [HttpGet("{ID}/tags")]
        public IActionResult GetTagOfProblem(string ID)
        {
            ProblemDTO problem = _problemService.GetProblemDetail(ID);
            if (problem == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Problem does not exist."
                });
            }
            return Ok(problem.Tags);
        }

        [HttpPost("{ID}/tags")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNewTagForProblem(string ID, ProblemTagInput data)
        {
            ProblemDTO problem = _problemService.GetProblemDetail(ID);
            if (problem == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Problem does not exist."
                });
            }
            string[] notExistTag = data.Tags.Where(x => _tagSerivce.FindByID(x) == null).ToArray();
            if (notExistTag.Length > 0)
            {
                return NotFound(new
                {
                    error = "Resource not found.",
                    detail = "Tag does not exist.",
                    value = notExistTag
                });
            }
            string[] duplicateTag = problem.Tags.Select(x => x.ID).Where(tag => data.Tags.Any(x => x == tag)).ToArray();
            if (duplicateTag.Length <= 0)
            {
                await _problemService.AddTag(ID, data.Tags);
                return Ok();
            }
            else
            {
                return Conflict(new
                {
                    error = "Conflict tag in problem.",
                    detail = "Tag already been added to problem.",
                    value = duplicateTag
                });
            }
        }

        [HttpDelete("{ID}/tags/{tagId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTagInProblem(string ID, string tagId)
        {
            ProblemDTO problem = _problemService.GetProblemDetail(ID);
            if (problem == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Problem does not exist."
                });
            }
            if (await _problemService.DeleteTag(ID, tagId))
            {
                return NoContent();
            }
            else
            {
                return BadRequest(new ErrorResponse
                {
                    error = "Invalid tag.",
                    detail = "Tag doesn't exist in problem."
                });
            }
        }
    }
}
