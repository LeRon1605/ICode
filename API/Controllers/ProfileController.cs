using API.Models.Entity;
using API.Repository;
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
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProfileController(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetProfile()
        {
            User user = _userRepository.FindSingle(user => user.ID == User.FindFirst("ID").Value);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<User, UserDTO>(user));
        }
        [HttpPut]
        public async Task<IActionResult> Update(UserUpdate input)
        {
            User user = _userRepository.FindSingle(user => user.ID == User.FindFirst("ID").Value);
            if (user == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(input.Username) && _userRepository.isExist(user => user.Username == input.Username))
            {
                return Conflict(new
                {
                    status = false,
                    message = "Username đã tồn tại không thể cập nhật"
                });
            }
            user.Username = input.Username;
            user.UpdatedAt = DateTime.Now;
            await _unitOfWork.CommitAsync();
            return Ok(_mapper.Map<User, UserDTO>(user));
        }
    }
}
