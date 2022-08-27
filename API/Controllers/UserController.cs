using API.Models.DTO;
using API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Filter;
using CodeStudy.Models;
using AutoMapper;
using API.Services;
using Microsoft.Extensions.Caching.Distributed;
using API.Extension;
using Data.Entity;

namespace API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IRoleService roleService, IMapper mapper)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
        }


        [HttpGet("search")]
        [QueryConstraint(Key = "page")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Find(int page, int pageSize , string keyword = "")
        {
            PagingList<User> list = await _userService.GetPageAsync(page, pageSize, keyword);
            return Ok(_mapper.Map<PagingList<User>, PagingList<UserDTO>>(list)); 
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromServices] IDistributedCache cache)
        {
            IEnumerable<UserDTO> users = await cache.GetRecordAsync<IEnumerable<UserDTO>>("users");
            bool isFromCache = true;
            if (users == null)
            {
                users = _mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(_userService.GetAll());
                await cache.SetRecordAsync("users", users);
                isFromCache = false;
            }
            return Ok(new
            {
                data = users,
                from = isFromCache ? "cache" : "db"
            });
        }
        [HttpGet("{ID}")]
        [Authorize]
        public IActionResult GetByID(string ID)
        {
            User user = _userService.FindByID(ID);
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
                return Ok(_mapper.Map<User, UserDTO>(user));
            }
        }

        [HttpPut("{ID}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string ID, UserUpdate input)
        {
            User user = _userService.FindByID(ID);
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
                });
            }
            await _userService.Update(ID, input);
            return Ok(_mapper.Map<User, UserDTO>(user));
        }

        [HttpDelete("{ID}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string ID)
        {
            User user = _userService.FindByID(ID);
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
                await _userService.Remove(ID);
                return NoContent();
            }
        }

        [HttpGet("{ID}/role")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetRole(string ID)
        {
            User user = _userService.FindByID(ID);
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
                return Ok(_mapper.Map<Role, RoleDTO>(_roleService.FindById(user.RoleID)));
            }
        }

        [HttpPut("{ID}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoleOfUser(string ID, RoleUpdate input)
        {
            User user = _userService.FindByID(ID);
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
                if (await _userService.UpdateRole(user, input.Name))
                {
                    return NoContent();
                }
                else
                {
                    return NotFound(new ErrorResponse
                    {
                        error = "Resource not found.",
                        detail = "Role does not exist."
                    });
                }
            }
        }

        [HttpGet("{ID}/submissions")]
        [Authorize]
        public IActionResult GetSubmitOfUser(string ID)
        {
            User user = _userService.FindByID(ID);
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
                return Ok(_mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_userService.GetSubmitOfUser(ID)));
            }
        }
    }
}
