using API.Filter;
using API.Helper;
using API.Models.DTO;
using API.Models.Entity;
using API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
        private readonly ICodeExecutor _codeExecutor;
        public ProblemController(IUnitOfWork unitOfWork, IProblemRepository problemRepository, ITagRepository tagRepository, ITestcaseRepository testcaseRepository, ISubmissionRepository submissionRepository, ICodeExecutor codeExecutor)
        {
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
            return Ok(_problemRepository.FindAll());
        }
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        public IActionResult Create(ProblemInput input)
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
                ArticleID = input.ArticleID,
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
            _problemRepository.Add(problem);
            _unitOfWork.Commit();
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
            return Ok(new
            {
                status = true,
                data = new ProblemDTO
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
                    Tags = problem.Tags.Select(x => new TagDTO { 
                        ID = x.ID,
                        Name = x.Name,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    }).ToList(),
                    CreatedAt = problem.CreatedAt,
                    UpdatedAt = problem.UpdatedAt
                }
            });
        }
        [HttpDelete("{ID}")]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        [ServiceFilter(typeof(ValidateIDAttribute))]
        public IActionResult Delete(string ID)
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
                _unitOfWork.Commit();
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
        public IActionResult Update(string ID, ProblemInputUpdate input)
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
                _unitOfWork.Commit();
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
        public IActionResult CreateTestCase(string ID, TestcaseInput input)
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
                _testcaseRepository.Add(testcase);
                _unitOfWork.Commit();
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
        public IActionResult UpdateTestcase(string ID, string TCID, TestcaseInput input)
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
                    _unitOfWork.Commit();
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
        public IActionResult DeleteTestcase(string ID, string TCID)
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
                    _unitOfWork.Commit();
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
                Console.WriteLine(result.output);
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
            _submissionRepository.Add(submission);
            _unitOfWork.Commit();
            return Ok(new 
            { 
                status = true,
                data = new
                {
                    status = submission.Status,
                    submitID = submission.ID,
                    details = submission.SubmissionDetails.Select(x => new
                    {
                        TestcaseID = x.TestCaseID,
                        Time = x.Time,
                        Memory = x.Memory,
                        Description = x.Description,
                        Status = x.Status
                    })
                }
            });
        }
    }
}
