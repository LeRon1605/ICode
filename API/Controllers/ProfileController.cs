using API.Filter;
using API.Helper;
using API.Models.DTO;
using API.Models.Entity;
using API.Services;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CodeStudy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpPost]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Update([FromForm] UserUpdate input, [FromForm] IFormFile avatar, [FromServices] IUploadService uploadService)
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
            if (_userService.Exist(input.Username, input.Username))
            {
                return Conflict(new ErrorResponse
                {
                    error = "Update failed.",
                    detail = "Username or email already exist."
                }); ;
            }
            if (avatar != null && avatar.Length > 0)
            {
                using (var stream = avatar.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(avatar.Name, stream),
                        Folder = "ICode"
                    };
                    string avtUrl = await uploadService.UploadAsync(uploadParams);
                    if (avtUrl == null)
                    {
                        throw new Exception("Upload file faild");
                    }
                    else
                    {
                        user.Avatar = avtUrl;
                    }
                }
            }
            await _userService.Update(user, input);
            return Ok(_mapper.Map<User, UserDTO>(user));
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
