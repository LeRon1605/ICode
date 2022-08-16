using API.Helper;
using API.Models.DTO;
using API.Models.Entity;
using API.Services;
using AutoMapper;
using CodeStudy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("me")]
    [Authorize]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISubmissionService _submissionService;
        private readonly IMapper _mapper;
        public ProfileController(IUserService userService, ISubmissionService submissionService,IMapper mapper)
        {
            _userService = userService;
            _submissionService = submissionService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetProfile()
        {
            User user = _userService.FindById(User.FindFirst(Constant.ID).Value);
            if (user == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "User does not exist."
                });
            }
            return Ok(_mapper.Map<User, UserDTO>(user));
        }

        [HttpPut]
        public async Task<IActionResult> Update(UserUpdate input)
        {
            User user = _userService.FindById(User.FindFirst(Constant.ID).Value);
            if (user == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "User does not exist."
                });
            }
            if (await _userService.Update(user, input))
            {
                return Ok(_mapper.Map<User, UserDTO>(user));
            }
            else
            {
                return Conflict(new ErrorResponse
                {
                    error = "Update failed.",
                    detail = "Username or email already exist."
                });
            }
        }

        [HttpGet("problems")]
        public IActionResult GetProblemOfUser()
        {
            User user = _userService.FindById(User.FindFirst(Constant.ID).Value);
            if (user == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "User does not exist."
                });
            }
            else
            {
                return Ok(_mapper.Map<IEnumerable<Problem>, IEnumerable<ProblemDTO>>(_userService.GetProblemCreatedByUser(User.FindFirst("ID").Value)));
            }
        }

        [HttpGet("submissions")]
        public IActionResult GetSubmitOfUser()
        {
            User user = _userService.FindById(User.FindFirst(Constant.ID).Value);
            if (user == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "User does not exist."
                });
            }
            else
            {
                return Ok(_mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_submissionService.GetSubmissionOfUsers(User.FindFirst("ID").Value)));
            }
        }
    }
}
