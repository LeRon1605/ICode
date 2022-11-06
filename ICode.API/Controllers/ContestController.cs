using API.Filter;
using Data.Entity;
using Hangfire;
using ICode.Common;
using ICode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using Services.Interfaces;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ICode.API.Controllers
{
    [Route("contests")]
    [ApiController]
    public class ContestController: ControllerBase
    {
        private readonly IContestService _contestService;
        private readonly IProblemService _problemService;
        public ContestController(IContestService contestService, IProblemService problemService)
        {
            _contestService = contestService;
            _problemService = problemService;
        }

        [HttpGet]
        [QueryConstraint(Key = "sort", Value = "name, state, date", Retrict = false)]
        [QueryConstraint(Key = "orderBy", Value = "asc, desc", Depend = "sort")]
        public IActionResult GetAll(int? page = null, int pageSize = 5, string name = "", bool? state = null, DateTime? date = null, string sort = "", string orderBy = "")
        {
            if (page == null)
            {
                return Ok(_contestService.GetContestByFilter(name, date, state, sort, orderBy));
            }
            else
            {
                return Ok(_contestService.GetPageContestByFilter(page.Value, pageSize, name, date, state, sort, orderBy));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateContest(ContestCreate input)
        {
            string[] invalidProblem = input.Problems.Where(x => _problemService.FindByID(x.ID) == null).Select(x => x.ID).ToArray();
            if (invalidProblem.Length > 0)
            {
                return BadRequest(new ErrorResponse
                {
                    error = "Problem doesn't exist.",
                    detail = invalidProblem
                });
            }
            Contest contest = new Contest
            {
                ID = Guid.NewGuid().ToString(),
                Name = input.Name,
                Description = input.Description,
                ProblemContestDetails = input.Problems.Select(x => new ProblemContestDetail
                {
                    ProblemID = x.ID,
                    Level = x.Level,
                    Score = x.Score
                }).ToList(),
                CreatedAt = DateTime.Now,
                StartAt = input.StartAt,
                EndAt = input.EndAt,
                PlayerLimit = input.PlayerLimit,
                UpdatedAt = null
            };
            await _contestService.Add(contest);
            if ((DateTime.Now - contest.StartAt).TotalDays < 3)
            {
                BackgroundJob.Enqueue<IContestService>(x => x.NotifyUser(contest.ID));
            }
            else
            {
                BackgroundJob.Schedule<IContestService>(x => x.NotifyUser(contest.ID), contest.StartAt.AddDays(-3));
            }
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            ContestBase contest = _contestService.GetDetailById(id);
            if (contest == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Contest doesn't exist."
                });
            }
            else
            {
                return Ok(contest);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateContest(string id, ContestUpdate input)
        {
            Contest contest = _contestService.FindByID(id);
            if (contest == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Contest doesn't exist."
                });
            }
            else
            {
                if (input.StartAt != null && ((input.EndAt != null && input.StartAt > input.EndAt) || (input.EndAt == null && input.StartAt > contest.EndAt)))
                {
                    return BadRequest(new ErrorResponse
                    {
                        error = "Invalid information.",
                        detail = "Start date of the contest is not allowed to greater than end date."
                    });
                }
                if (input.EndAt != null && (input.EndAt < contest.StartAt && input.StartAt == null) || (input.EndAt < input.StartAt && input.StartAt != null))
                {
                    return BadRequest(new ErrorResponse
                    {
                        error = "Invalid information.",
                        detail = "End date of the contest is not allowed to sooner than start date."
                    });
                }
                if (input.Problems != null && input.Problems.Length > 0)
                {
                    string[] invalidProblem = input.Problems.Where(x => _problemService.FindByID(x.ID) == null).Select(x => x.ID).ToArray();
                    if (invalidProblem.Length > 0)
                    {
                        return BadRequest(new ErrorResponse
                        {
                            error = "Problem doesn't exist.",
                            detail = invalidProblem
                        });
                    }
                }
                await _contestService.Update(id, input);
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteContest(string id)
        {
            Contest contest = _contestService.FindByID(id);
            if (contest == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Contest doesn't exist."
                });
            }
            else
            {
                if (contest.StartAt >= DateTime.Now || contest.EndAt <= DateTime.Now)
                {
                    await _contestService.Remove(id);
                    return NoContent();
                }
                else
                {
                    return BadRequest(new ErrorResponse
                    {
                        error = "Can not remove this contest.",
                        detail = "You can not remove contest running."
                    });
                }
            }
        }
    }
}
