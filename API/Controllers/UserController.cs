using API.Models.DTO;
using API.Models.Entity;
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
        [QueryConstraint(Key = "pageSize")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Find(int page, int pageSize, string keyword = "")
        {
            PagingList<User> list = await _userService.GetPageAsync(page, pageSize, keyword);
            return Ok(_mapper.Map<PagingList<User>, PagingList<UserDTO>>(list)); 
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(_userService.GetAll()));
        }
        [HttpGet("{ID}")]
        [Authorize]
        [QueryConstraint(Key = "ID")]
        public IActionResult GetByID(string ID)
        {
            User user = _userService.FindById(ID);
            if (user == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy User"
                });
            }
            else
            {
                return Ok(_mapper.Map<User, UserDTO>(user));
            }
        }

        [HttpPut("{ID}")]
        [Authorize(Roles = "Admin")]
        [QueryConstraint(Key = "ID")]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Update(string ID, UserUpdate input)
        {
            User user = _userService.FindById(ID);
            if (user == null)
            {
                return NotFound(new
                {
                    message = "Không tồn tại user"
                });
            }
            if (await _userService.Update(user, input))
            {
                return Ok(_mapper.Map<User, UserDTO>(user));
            }
            else
            {
                return Conflict(new
                {
                    message = "Username đã tồn tại không thể cập nhật"
                });
            }

        }

        [HttpDelete("{ID}")]
        [Authorize(Roles = "Admin")]
        [QueryConstraint(Key = "ID")]
        [ServiceFilter(typeof(ExceptionHandler))]
        public async Task<IActionResult> Delete(string ID)
        {
            User user = _userService.FindById(ID);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                await _userService.Remove(user);
                return NoContent();
            }
        }

        [HttpGet("{ID}/role")]
        [Authorize(Roles = "Admin")]
        [QueryConstraint(Key = "ID")]
        public IActionResult GetRole(string ID)
        {
            User user = _userService.FindById(ID);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<Role, RoleDTO>(_roleService.FindById(user.RoleID)));
            }
        }

        [HttpPut("{ID}/role")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(ExceptionHandler))]
        [QueryConstraint(Key = "ID")]
        public async Task<IActionResult> UpdateRoleOfUser(string ID, RoleUpdate input)
        {
            User user = _userService.FindById(ID);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                if (await _userService.UpdateRole(user, input.Name))
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpGet("{ID}/submissions")]
        [Authorize]
        [QueryConstraint(Key = "ID")]
        public IActionResult GetSubmitOfUser(string ID)
        {
            User user = _userService.FindById(ID);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<IEnumerable<Submission>, IEnumerable<SubmissionDTO>>(_userService.GetSubmitOfUser(ID)));
            }
        }
    }
}
