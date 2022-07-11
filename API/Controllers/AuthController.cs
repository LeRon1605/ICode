using API.Helper;
using API.Models.DTO;
using API.Models.Entity;
using API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenProvider _tokenProvider;
        public AuthController(TokenProvider tokenProvider, IUnitOfWork unitOfWork, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }
        [HttpPost("register")]
        public IActionResult Register(RegisterUser input)
        {
            User user = new User
            {
                ID = Guid.NewGuid().ToString(),
                Email = input.Email,
                Password = input.Password,
                Username = input.Username,
                CreatedAt = DateTime.Now,
                Role = _roleRepository.findByName("User")
            };
            if (_userRepository.isExist(x => x.Username == user.Username || x.Email == user.Email))
            {
                return Conflict(new
                {
                    status = false,
                    message = "Tên tài khoản hoặc email đã tồn tại"
                });
            }
            else
            {
                try
                {
                    _userRepository.Add(user);
                    _unitOfWork.Commit();
                    return CreatedAtAction("GetByID", "User", new { id = user.ID }, new { status = true, message = "Đăng kí tài khoản thành công"});
                }
                catch (Exception e)
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = e.Message
                    });
                }
            }
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser input)
        {
            return null;
        }
    }
}
