using API.Filter;
using API.Models.Entity;
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
    [Route("testcases")]
    [ApiController]
    public class TestcaseController : ControllerBase
    {
        private readonly ITestcaseService _testcaseService;
        private readonly IProblemService _problemService;
        private readonly IMapper _mapper;
        public TestcaseController(ITestcaseService testcaseService, IProblemService problemService, IMapper mapper)
        {
            _testcaseService = testcaseService;
            _problemService = problemService;
            _mapper = mapper;
        }
        [HttpGet("{ID}")]
        [Authorize]
        public IActionResult GetTestcaseByID(string ID)
        {
            TestCase testcase = _testcaseService.FindById(ID);
            if (testcase == null)
            {
                return NotFound();
            }
            else
            {
                Problem problem = _problemService.FindByID(testcase.ProblemID);
                if (problem == null)
                {
                    return NotFound();
                }
                if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
                {
                    return Ok(_mapper.Map<TestCase, TestcaseDTO>(testcase));
                }
                else
                {
                    return Forbid();
                }

            }
        }
        [HttpPut("{ID}")]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> UpdateTestcase(string ID, TestcaseInput input)
        {
            TestCase testcase = _testcaseService.FindById(ID);
            if (testcase == null)
            {
                return NotFound();
            }
            else
            {
                Problem problem = _problemService.FindByID(testcase.ProblemID);
                if (problem == null)
                {
                    return NotFound();
                }
                if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
                {
                    await _testcaseService.Update(ID, input);
                    return NoContent();
                }
                else
                {
                    return Forbid();
                }
            }
        }
        [HttpDelete("{ID}")]
        [Authorize]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> DeleteTestcase(string ID)
        {
            TestCase testcase = _testcaseService.FindById(ID);
            if (testcase == null)
            {
                return NotFound();
            }
            else
            {
                Problem problem = _problemService.FindByID(testcase.ProblemID);
                if (problem == null)
                {
                    return NotFound();
                }
                if (problem.ArticleID == User.FindFirst("ID")?.Value || User.FindFirst("Role")?.Value == "Admin")
                {
                    await _testcaseService.Remove(ID);
                    return NoContent();
                }
                else
                {
                    return Forbid();
                }
            }
        }
    }
}
