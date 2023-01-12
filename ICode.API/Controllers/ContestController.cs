using API.Extension;
using API.Filter;
using API.Services;
using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Hangfire;
using ICode.API.Mapper.ContestMapper;
using ICode.Common;
using ICode.Models;
using ICode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using Models.DTO;
using Services;
using Services.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
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
        private readonly ISubmissionService _submissionService;
        private readonly IDistributedCache _cache;
        public ContestController(IContestService contestService, IProblemService problemService, ISubmissionService submissionService, IDistributedCache cache)
        {
            _contestService = contestService;
            _problemService = problemService;
            _submissionService = submissionService;
            _cache = cache;
        }

        [HttpGet]
        [QueryConstraint(Key = "sort", Value = "name, state, date", Retrict = false)]
        [QueryConstraint(Key = "orderBy", Value = "asc, desc", Depend = "sort")]
        public IActionResult GetAll([FromServices] IMapper mapper,int? page = null, int pageSize = 5, string name = "", bool? state = null, DateTime? date = null, string sort = "", string orderBy = "")
        {
            IContestMapper contestMapper = new LimitContestMapper(mapper);
            if (User.Identity.IsAuthenticated && User.FindFirst(Constant.ROLE).Value == "Admin")
            {
                contestMapper = new RunningContestMapper(mapper);
            }
            if (page == null)
            {
                return Ok(_contestService.GetContestByFilter(name, date, state, sort, orderBy, contestMapper));
            }
            else
            {
                return Ok(_contestService.GetPageContestByFilter(page.Value, pageSize, name, date, state, sort, orderBy, contestMapper));
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
            if ((contest.StartAt - DateTime.Now).TotalDays < 3)
            {
                BackgroundJob.Enqueue<IContestService>(x => x.NotifyUser(contest.ID));
            }
            else
            {
                string jobID = BackgroundJob.Schedule<IContestService>(x => x.NotifyUser(contest.ID), contest.StartAt.AddDays(-3));
                await _cache.SetRecordAsync($"contest_notify_{contest.ID}", jobID, TimeSpan.FromDays((contest.StartAt.AddDays(-3) - DateTime.Now).TotalDays));
            }
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromServices] IMapper mapper, string id)
        {
            IContestMapper contestMapper = new LimitContestMapper(mapper);
            if (User.Identity.IsAuthenticated && User.FindFirst(Constant.ROLE).Value == "Admin")
            {
                contestMapper = new RunningContestMapper(mapper);
            }
            ContestBase contest = _contestService.GetDetailById(id, contestMapper);
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
                if (input.StartAt != null)
                {
                    string jobID = await _cache.GetRecordAsync<string>($"contest_notify_{id}");
                    if (jobID != null)
                    {
                        BackgroundJob.Delete(jobID);
                    }
                    if ((input.StartAt.Value - DateTime.Now).TotalDays < 3)
                    {
                        BackgroundJob.Enqueue<IContestService>(x => x.NotifyUser(id));
                    }
                    else
                    {
                        jobID = BackgroundJob.Schedule<IContestService>(x => x.NotifyUser(id), input.StartAt.Value.AddDays(-3));
                        await _cache.SetRecordAsync($"contest_notify_{id}", jobID, TimeSpan.FromDays((contest.StartAt.AddDays(-3) - DateTime.Now).TotalDays));
                    }
                }
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
                    string jobID = await _cache.GetRecordAsync<string>($"contest_notify_{id}");
                    if (jobID != null)
                    {
                        BackgroundJob.Delete(jobID);
                    }
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

        [HttpGet("{id}/players")]
        [QueryConstraint(Key = "sort", Value = "name, gender, date", Retrict = false)]
        [QueryConstraint(Key = "orderBy", Depend = "sort", Value = "asc, desc")]
        public IActionResult GetAllPlayers(string id, int? page = null, int pageSize = 5, string name = "", bool? gender = null, DateTime? date = null, string sort = "", string orderBy = "")
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
            if (page == null)
            {
                return Ok(_contestService.GetPlayerOfContest(id, name, gender, date, sort, orderBy));
            }
            else
            {
                return Ok(_contestService.GetPagePlayerOfContestByFilter(id, page.Value, pageSize, name, gender, date, sort, orderBy));
            }
        }

        [HttpPost("{id}/players")]
        [Authorize]
        public async Task<IActionResult> RegisterContest(string id)
        {
            ServiceResult result = await _contestService.Register(id, User.FindFirst(Constant.ID).Value);
            if (result.State == ServiceState.EntityNotFound)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = result.Message
                });
            }
            else if (result.State == ServiceState.InvalidAction)
            {
                return BadRequest(new ErrorResponse
                {
                    error = "Invalid action.",
                    detail = result.Message
                });
            }
            return Ok();
        }

        [HttpPost("{id}/players/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterContest(string id, string userId, [FromServices] IUserService userService)
        {
            ServiceResult result = await _contestService.Register(id, userId);
            if (result.State == ServiceState.EntityNotFound)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = result.Message
                });
            }
            else if (result.State == ServiceState.InvalidAction)
            {
                return BadRequest(new ErrorResponse
                {
                    error = "Invalid action.",
                    detail = result.Message
                });
            }
            return Ok();
        }

        [HttpDelete("{id}/players")]
        [Authorize]
        public async Task<IActionResult> DiscardContest(string id)
        {
            ServiceResult result = await _contestService.RemoveUser(id, User.FindFirst(Constant.ID).Value);
            if (result.State == ServiceState.EntityNotFound)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = result.Message
                });
            }
            else if (result.State == ServiceState.InvalidAction)
            {
                return BadRequest(new ErrorResponse
                {
                    error = "Invalid action.",
                    detail = result.Message
                });
            }
            return Ok();
        }

        [HttpDelete("{id}/players/{userId}")]
        [Authorize]
        public async Task<IActionResult> DiscardContest(string id, string userId, [FromServices] IUserService userService)
        {
            ServiceResult result = await _contestService.RemoveUser(id, userId);
            if (result.State == ServiceState.EntityNotFound)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = result.Message
                });
            }
            else if (result.State == ServiceState.InvalidAction)
            {
                return BadRequest(new ErrorResponse
                {
                    error = "Invalid action.",
                    detail = result.Message
                });
            }
            return Ok(result.Data);
        }

        [HttpGet("{id}/submissions")]
        public IActionResult GetSubmissions(string id)
        {
            ServiceResult result = _contestService.GetSubmissions(id);
            if (result.State == ServiceState.EntityNotFound)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = result.Message
                });
            }
            return Ok(result.Data);
        }

        [HttpPost("{id}/submissions")]
        [Authorize]
        public async Task<IActionResult> SubmitContest(string id, SubmissionContestInput input)
        {
            Contest contest = _contestService.FindByID(id);
            if (contest == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Contest not found.",
                    detail = "Contest doesn't exist."
                });
            }
            if (!_contestService.IsUserInContest(id, User.FindFirst(Constant.ID).Value))
            {
                return BadRequest(new ErrorResponse
                {
                    error = "Invalid action.",
                    detail = "You are not in the contest."
                });
            }
            if (!_contestService.IsProblemInContest(id, input.ProblemID))
            {
                return BadRequest(new ErrorResponse
                {
                    error = "Invalid action.",
                    detail = "Problem is not in the contest."
                });
            }

            Submission submission = new Submission
            {
                ID = Guid.NewGuid().ToString(),
                State = SubmitState.Pending,
                UserID = User.FindFirst(Constant.ID).Value,
                Code = input.Code,
                Language = input.Language,
                CreatedAt = DateTime.Now,
                ProblemID = input.ProblemID,
                ContestSubmission = new ContestSubmission
                {
                    Contest = contest
                },
                SubmissionDetails = new List<SubmissionDetail>()
            };

            SubmissionResult submissionResult;

            if (!_contestService.IsUserSolvedProblem(id, User.FindFirst(Constant.ID).Value, input.ProblemID))
            {
                submissionResult = await _submissionService.Submit(submission);
                if (submissionResult.Status)
                {
                    await _contestService.AddPointForUser(id, User.FindFirst(Constant.ID).Value, input.ProblemID);
                }
            }
            else
            {
                submissionResult = await _submissionService.Submit(submission);
            }
            return Ok(submissionResult);
        }
    }
}
