﻿using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using ICode.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Services.Interfaces;
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
            TestCase testcase = _testcaseService.FindByID(ID);
            if (testcase == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Testcase does not exist."
                });
            }
            else
            {
                Problem problem = _problemService.FindByID(testcase.ProblemID);
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
        public async Task<IActionResult> UpdateTestcase(string ID, TestcaseUpdate input)
        {
            TestCase testcase = _testcaseService.FindByID(ID);
            if (testcase == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Testcase does not exist."
                });
            }
            else
            {
                Problem problem = _problemService.FindByID(testcase.ProblemID);
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
        public async Task<IActionResult> DeleteTestcase(string ID)
        {
            TestCase testcase = _testcaseService.FindByID(ID);
            if (testcase == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Testcase does not exist."
                });
            }
            else
            {
                Problem problem = _problemService.FindByID(testcase.ProblemID);
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
