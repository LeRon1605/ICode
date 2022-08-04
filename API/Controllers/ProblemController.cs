using API.Filter;
using API.Helper;
using API.Models.DTO;
using API.Models.Entity;
using API.Repository;
using CodeStudy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("problems")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProblemRepository _problemRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ITestcaseRepository _testcaseRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IReportRepository _reportRepository;
        private readonly ICodeExecutor _codeExecutor;
        public ProblemController(IUnitOfWork unitOfWork, IProblemRepository problemRepository, ITagRepository tagRepository, ITestcaseRepository testcaseRepository, ISubmissionRepository submissionRepository, ICodeExecutor codeExecutor, IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
            _unitOfWork = unitOfWork;
            _problemRepository = problemRepository;
            _tagRepository = tagRepository;
            _submissionRepository = submissionRepository;
            _testcaseRepository = testcaseRepository;
            _codeExecutor = codeExecutor;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_problemRepository.GetProblemDetailMulti().Select(x => new  
            { 
                ID = x.ID,
                Description = x.Description,
                Name = x.Name,
                Article = new UserDTO
                {
                    ID = x.Article.ID,
                    Username = x.Article.Username,
                    Email = x.Article.Email,
                    CreatedAt = x.Article.CreatedAt,
                    UpdatedAt = x.Article.UpdatedAt
                },
                Tags = x.Tags.Select(tag => new TagDTO { 
                   ID = tag.ID,
                   Name = tag.Name,
                   CreatedAt = tag.CreatedAt,
                   UpdatedAt = tag.UpdatedAt
                }).ToList(),
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }));
        }
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Create(ProblemInput input)
        {
            if (input.Tags.Any(x => !_tagRepository.isExist(tag => tag.ID == x)))
            {
                return NotFound(new
                {
                    status = false,
                    message = "Tag not found"
                });
            }    
            Problem problem = new Problem
            {
                ID = Guid.NewGuid().ToString(),
                Name = input.Name,
                Status = false,
                Description = input.Description,
                ArticleID = User.FindFirst("ID")?.Value,
                CreatedAt = DateTime.Now,
                TestCases = input.TestCases.Select(x => new TestCase
                {
                    ID = Guid.NewGuid().ToString(),
                    Input = x.Input,
                    Output = x.Output,
                    CreatedAt = DateTime.Now,
                    MemoryLimit = x.MemoryLimit,
                    TimeLimit = x.TimeLimit,
                }).ToList(),
                Tags = input.Tags.Select(x => _tagRepository.FindSingle(tag => tag.ID == x)).ToList()
            };
            await _problemRepository.AddAsync(problem);
            await _unitOfWork.CommitAsync();
            return CreatedAtAction("GetByID", "Problem", new { ID = problem.ID }, new { status = true, message = "Tạo mới problem thành công" });
        }
        [HttpGet("{ID}")]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        public IActionResult GetByID(string ID)
        {
            Problem problem = _problemRepository.GetProblemDetail(x => x.ID == ID);
            if (problem == null)
            {
                return NotFound(new
                {
                    status = false, 
                    message = "Problem Not Found"
                });
            }
            return Ok(new ProblemDTO
            {
                ID = problem.ID,
                Description = problem.Description,
                Name = problem.Name,
                Article = new UserDTO
                {
                    ID = problem.Article.ID,
                    Username = problem.Article.Username,
                    Email = problem.Article.Email,
                    CreatedAt = problem.Article.CreatedAt,
                    UpdatedAt = problem.Article.UpdatedAt
                },
                Tags = problem.Tags.Select(x => new TagDTO
                {
                    ID = x.ID,
                    Name = x.Name,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                }).ToList(),
                CreatedAt = problem.CreatedAt,
                UpdatedAt = problem.UpdatedAt
            });
        }
        [HttpDelete("{ID}")]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        public async Task<IActionResult> Delete(string ID)
        {
            Problem problem = _problemRepository.FindSingle(x => x.ID == ID);
            if (problem == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Problem not found"
                });
            }
            if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
            {
                _problemRepository.Remove(problem);
                await _unitOfWork.CommitAsync();
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
        [ServiceFilter(typeof(ValidateIDAttribute))]
        public async Task<IActionResult> Update(string ID, ProblemInputUpdate input)
        {
            Problem problem = _problemRepository.FindSingle(x => x.ID == ID);
            if (problem == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Problem not found"
                });
            }
            if (input.Tags.Any(x => !_tagRepository.isExist(tag => tag.ID == x)))
            {
                return NotFound(new
                {
                    status = false,
                    message = "Tag not found"
                });
            }
            if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
            {
                problem.Name = input.Name;
                problem.Description = input.Description;
                problem.Status = input.Status;
                problem.Tags = input.Tags.Select(x => _tagRepository.FindSingle(tag => tag.ID == x)).ToList();
                problem.UpdatedAt = DateTime.Now;
                _problemRepository.Update(problem);
                await _unitOfWork.CommitAsync();
                return NoContent();
            }
            else
            {
                return Forbid();
            }
        }
        [HttpGet("{ID}/testcases")]
        [Authorize]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        public IActionResult GetTestCases(string ID)
        {
            Problem problem = _problemRepository.GetProblemWithTestcase(x => x.ID == ID);
            if (problem == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Problem not found"
                });
            }
            if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
            {
                return Ok(new
                {
                    status = true,
                    data = new
                    {
                        problemID = ID,
                        testcases = problem.TestCases.Select(testcase => new TestcaseDTO
                        {
                            ID = testcase.ID,
                            Input = testcase.Input,
                            CreatedAt = testcase.CreatedAt,
                            MemoryLimit = testcase.MemoryLimit,
                            Output = testcase.Output,
                            TimeLimit = testcase.TimeLimit,
                            UpdatedAt = testcase.UpdatedAt
                        })
                    }
                });
            }
            else
            {
                return Forbid();
            }
        }
        [HttpPost("{ID}/testcases")]
        [Authorize]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> CreateTestCase(string ID, TestcaseInput input)
        {
            Problem problem = _problemRepository.GetProblemWithTestcase(x => x.ID == ID);
            if (problem == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Problem not found"
                });
            }
            if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
            {
                TestCase testcase = new TestCase
                {
                    ID = Guid.NewGuid().ToString(),
                    Input = input.Input,
                    Output = input.Output,
                    CreatedAt = DateTime.Now,
                    MemoryLimit = input.MemoryLimit,
                    TimeLimit = input.TimeLimit,
                    ProblemID = ID
                };
                await _testcaseRepository.AddAsync(testcase);
                await _unitOfWork.CommitAsync();
                return CreatedAtAction("GetTestcaseByID", "Problem", new { ID = ID, TCID = testcase.ID }, new { status = true, message = "Tạo testcase thành công" });
            }
            else
            {
                return Forbid();
            }
        }
        [HttpGet("{ID}/testcases/{TCID}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        public IActionResult GetTestcaseByID(string ID, string TCID)
        {
            Problem problem = _problemRepository.GetProblemWithTestcase(x => x.ID == ID);
            if (problem == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Problem not found"
                });
            }
            if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
            {
                TestCase testcase = problem.TestCases.FirstOrDefault(x => x.ID == TCID);
                if (testcase == null)
                {
                    return NotFound(new
                    {
                        status = false,
                        message = "Testcase not found"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = true,
                        data = new
                        {
                            problemID = ID,
                            testcase = new TestcaseDTO
                            {
                                ID = testcase.ID,
                                Input = testcase.Input,
                                CreatedAt = testcase.CreatedAt,
                                MemoryLimit = testcase.MemoryLimit,
                                Output = testcase.Output,
                                TimeLimit = testcase.TimeLimit,
                                UpdatedAt = testcase.UpdatedAt
                            }
                        }
                    });
                }   
            }
            else
            {
                return Forbid();
            }
        }
        [HttpPut("{ID}/testcases/{TCID}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> UpdateTestcase(string ID, string TCID, TestcaseInput input)
        {
            Problem problem = _problemRepository.FindSingle(x => x.ID == ID);
            if (problem == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Problem not found"
                });
            }
            if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
            {
                TestCase testcase = _testcaseRepository.FindSingle(x => x.ID == TCID);
                if (testcase == null)
                {
                    return NotFound(new
                    {
                        status = false,
                        message = "Testcase not found"
                    });
                }
                else
                {
                    testcase.Input = input.Input;
                    testcase.Output = input.Output;
                    testcase.MemoryLimit = input.MemoryLimit;
                    testcase.TimeLimit = input.TimeLimit;
                    _testcaseRepository.Update(testcase);
                    await _unitOfWork.CommitAsync();
                    return NoContent();
                }
            }
            else
            {
                return Forbid();
            }
        }
        [HttpDelete("{ID}/testcases/{TCID}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> DeleteTestcase(string ID, string TCID)
        {
            Problem problem = _problemRepository.GetProblemWithTestcase(x => x.ID == ID);
            if (problem == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Problem not found"
                });
            }
            if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
            {
                TestCase testcase = problem.TestCases.FirstOrDefault(x => x.ID == TCID);
                if (testcase == null)
                {
                    return NotFound(new
                    {
                        status = false,
                        message = "Testcase not found"
                    });
                }
                else
                {
                    _testcaseRepository.Remove(testcase);
                    await _unitOfWork.CommitAsync();
                    return NoContent();
                }
            }
            else
            {
                return Forbid();
            }
        }
        [HttpPost("{ID}/submissions")]
        [Authorize]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Submit(string ID, SubmissionInput input)
        {
            Problem problem = _problemRepository.GetProblemWithTestcase(x => x.ID == ID);
            if (problem == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Problem not found"
                });
            }
            Submission submission = new Submission
            {
                ID = Guid.NewGuid().ToString(),
                Status = false,
                UserID = User.FindFirst("ID")?.Value,
                Code = input.Code,
                Language = input.Language,
                CreatedAt = DateTime.Now,
                SubmissionDetails = new List<SubmissionDetail>()
            };
            foreach (TestCase testcase in problem.TestCases)
            {
                ExecutorResult result = await _codeExecutor.ExecuteCode(input.Code, input.Language, testcase.Input);
                SubmissionDetail submitDetail = new SubmissionDetail
                {
                    Memory = Convert.ToSingle(result.memory),
                    Time = Convert.ToSingle(result.cpuTime),
                    Status = false,
                    TestCaseID = testcase.ID,
                };
                if (result.output == testcase.Output)
                {
                    if (Convert.ToSingle(result.cpuTime) <= testcase.TimeLimit)
                    {
                        if (Convert.ToSingle(result.memory) <= testcase.MemoryLimit)
                        {
                            submitDetail.Description = "Success";
                            submitDetail.Status = true;
                            submission.Status = true;
                        }
                        else
                        {
                            submitDetail.Description = "Memory Limit";
                        }
                    }
                    else
                    {
                        submitDetail.Description = "Time Limit";
                    }
                }
                else
                {
                    submitDetail.Description = "Wrong Answer";
                }
                submission.SubmissionDetails.Add(submitDetail);
            }
            await _submissionRepository.AddAsync(submission);
            await _unitOfWork.CommitAsync();
            return Ok(new SubmissionDTO { 
                ID = submission.ID,
                Status = submission.Status,
                Code = submission.Code,
                CreatedAt = submission.CreatedAt,
                Language = submission.Language,
                UserID = submission.UserID
            });
        }
        [HttpGet("{ID}/submissions")]
        [Authorize]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        public IActionResult GetSubmitOfProblem(string ID)
        {
            Problem problem = _problemRepository.FindSingle(x => x.ID == ID);
            if (problem == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Problem not found"
                });
            }
            List<Submission> submissions;
            if (User.FindFirst("Role")?.Value == "User")
            {
                string userID = User.FindFirst("ID")?.Value;
                submissions = _submissionRepository.GetSubmissionsDetail(x => x.UserID == userID).ToList();
            }
            else
            {
                submissions = _submissionRepository.GetSubmissionsDetail(x => x.SubmissionDetails.Where(detail => detail.TestCase.ProblemID == ID).Select(detail => detail.SubmitID).Contains(x.ID)).ToList();
            }
            return Ok(new
            {
                status = true,
                data = submissions.Select(x => new
                {
                    ID = x.ID,
                    Code = x.Code,
                    Language = x.Language,
                    Status = x.Status,
                    UserID = x.UserID,
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

        [HttpPost("{ID}/reports")]
        [Authorize(Roles = "User")]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Report(string ID, ReportInput input)
        {
            Problem problem = _problemRepository.FindSingle(x => x.ID == ID);
            if (problem == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Problem not found"
                });
            }
            Report report = new Report
            {
                ID = Guid.NewGuid().ToString(),
                Content = input.Content,
                Title = input.Title,
                ProblemID = ID,
                UserID = User.FindFirst("ID")?.Value,
                CreatedAt = DateTime.Now,
            };
            await _reportRepository.AddAsync(report);
            await _unitOfWork.CommitAsync();
            return Ok();
        }
    }
}
