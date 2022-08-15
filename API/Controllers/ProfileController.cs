using API.Models.Entity;
using API.Repository;
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
            User user = _userService.FindById(User.FindFirst("ID").Value);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<User, UserDTO>(user));
        }
        [HttpPut]
        public async Task<IActionResult> Update(UserUpdate input)
        {
            User user = _userService.FindById(User.FindFirst("ID").Value);
            if (user == null)
            {
                return NotFound();
            }
            await _userService.Update(user, input);
            return Ok(_mapper.Map<User, UserDTO>(user));
        }

        [HttpGet("problems")]
        public IActionResult GetProblemOfUser()
        {
            User user = _userService.FindById(User.FindFirst("ID").Value);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<IEnumerable<Problem>, IEnumerable<ProblemDTO>>(_userService.GetProblemCreatedByUser(User.FindFirst("ID").Value)));
            }
        }

        [HttpGet("submissions")]
        public IActionResult GetSubmitOfUser()
        {
            User user = _userService.FindById(User.FindFirst("ID").Value);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_submissionService.GetSubmissionOfUsers(User.FindFirst("ID").Value)));
            }
        }
    }
}
