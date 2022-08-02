using API.Filter;
using API.Helper;
using API.Models.Entity;
using API.Repository;
using CodeStudy.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly IMail _mail;
        public AuthController(TokenProvider tokenProvider, IUnitOfWork unitOfWork, IUserRepository userRepository, IRoleRepository roleRepository, IMail mail)
        {
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mail = mail;
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetUser()
        {
            User user = _userRepository.GetUserWithRole(x => x.ID == User.FindFirst("ID").Value);
            if (user == null) return NotFound();
            return Ok(new
            {
                ID = user.ID,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Role = user.Role.Name
            });
        }
        [HttpPost("register")]
        [ServiceFilter(typeof(ExceptionHandler))]
        public IActionResult Register(RegisterUser input)
        {
            User user = new User
            {
                ID = Guid.NewGuid().ToString(),
                Email = input.Email,
                Password = Encryptor.MD5Hash(input.Password),
                Username = input.Username,
                CreatedAt = DateTime.Now,
                Role = _roleRepository.FindSingle(role => role.Name == "User")
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
                _userRepository.Add(user);
                _unitOfWork.Commit();
                return CreatedAtAction("GetByID", "User", new { id = user.ID }, new { status = true, message = "Đăng kí tài khoản thành công" });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser input)
        {
            User user = _userRepository.GetUserWithRole(user => (user.Username == input.Name || user.Email == input.Name) && Encryptor.MD5Hash(user.Password) == input.Password);
            if (user == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Tài khoản không tồn tại"
                });
            }    
            return Ok(new 
            { 
                status = true,
                message = "Đăng nhập thành công",
                userId = user.ID,
                token = _tokenProvider.GenerateToken(user)
            });
        }

        [HttpPost("forget-password")]
        public IActionResult ForgetPassword(string Name)
        {
            User user = _userRepository.FindSingle(user => user.Username == Name || user.Email == Name);
            if (user == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Không tìm thấy user"
                });
            }
            string token = "Không tìm thấy user";
            //await _mail.SendMailAsync(user.Email, "Đặt lại mật khẩu", "Bấm vào <a>đây</a>")
            return Ok(new 
            { 
                status = true,
                token = token
            });
        }

        [HttpGet("google")]
        public IActionResult GoogleAuth()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GetGoogleAuth") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

    }
}
