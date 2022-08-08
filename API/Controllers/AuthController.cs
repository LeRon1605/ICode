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
using Models;
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
        private readonly ITokenRepository _tokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenProvider _tokenProvider;
        private readonly IMail _mail;
        public AuthController(TokenProvider tokenProvider, IUnitOfWork unitOfWork, IUserRepository userRepository, IRoleRepository roleRepository, ITokenRepository tokenRepository, IMail mail)
        {
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
            _tokenRepository = tokenRepository;
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
        public async Task<IActionResult> Register(RegisterUser input)
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
                await _userRepository.AddAsync(user);
                await _unitOfWork.CommitAsync();
                return CreatedAtAction("GetByID", "User", new { id = user.ID }, new { status = true, message = "Đăng kí tài khoản thành công" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUser input)
        {
            string passwordHashed = Encryptor.MD5Hash(input.Password);
            User user = _userRepository.GetUserWithRole(user => (user.Username == input.Name || user.Email == input.Name) && passwordHashed == user.Password);
            if (user == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Tài khoản không tồn tại"
                });
            }
            AccessToken token = _tokenProvider.GenerateToken(user);
            RefreshToken refreshToken = new RefreshToken
            {
                ID = Guid.NewGuid().ToString(),
                UserID = user.ID,
                State = false,
                Token = _tokenProvider.GenerateRandomToken(),
                JwtID = token.ID,
                ExpiredAt = DateTime.Now.AddHours(6),
                IssuedAt = DateTime.Now
            };
            await _tokenRepository.AddAsync(refreshToken);
            await _unitOfWork.CommitAsync();
            return Ok(new 
            { 
                status = true,
                message = "Đăng nhập thành công",
                userId = user.ID,
                token = token.Token,
                refreshToken = refreshToken.Token
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RenewToken(Token token)
        {
            string jwtId = "";
            if (!_tokenProvider.ValidateToken(token.AccessToken, ref jwtId))
            {
                return BadRequest(new 
                { 
                    message = "Invalid Token"
                });
            }
            RefreshToken refreshToken = _tokenRepository.FindSingle(x => x.Token == token.RefreshToken);
            if (refreshToken == null || refreshToken.State || refreshToken.JwtID != jwtId)
            {
                return BadRequest(new
                {
                    message = "Invalid Refresh Token"
                });
            }
            User user = _userRepository.GetUserWithRole(user => user.ID == refreshToken.UserID);
            if (user == null)
            {
                return NotFound();
            }
            refreshToken.State = true;
            AccessToken newToken = _tokenProvider.GenerateToken(user);
            RefreshToken newRefreshToken = new RefreshToken
            {
                ID = Guid.NewGuid().ToString(),
                UserID = user.ID,
                State = false,
                Token = _tokenProvider.GenerateRandomToken(),
                JwtID = newToken.ID,
                ExpiredAt = DateTime.Now.AddHours(6),
                IssuedAt = DateTime.Now
            };
            _tokenRepository.Update(refreshToken);
            await _tokenRepository.AddAsync(newRefreshToken);
            await _unitOfWork.CommitAsync();
            return Ok(new
            {
                status = true,
                userId = user.ID,
                token = newToken.Token,
                refreshToken = newRefreshToken.Token
            }); 
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPassword input)
        {
            User user = _userRepository.FindSingle(user => user.Username == input.Name || user.Email == input.Name);
            if (user == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Không tìm thấy user"
                });
            }
            if (user.ForgotPasswordToken != null)
            {
                if (user.ForgotPasswordTokenExpireAt < DateTime.Now)
                {
                    user.ForgotPasswordToken = _tokenProvider.GenerateRandomToken();
                    user.ForgotPasswordTokenCreatedAt = DateTime.Now;
                    user.ForgotPasswordTokenExpireAt = DateTime.Now.AddHours(6);
                    _userRepository.Update(user);
                    _unitOfWork.Commit();
                }
                else
                {
                    return Conflict(new
                    {
                        status = false,
                        message = "Token còn hạn"
                    });
                }
            }
            else
            {
                user.ForgotPasswordToken = _tokenProvider.GenerateRandomToken();
                user.ForgotPasswordTokenCreatedAt = DateTime.Now;
                user.ForgotPasswordTokenExpireAt = DateTime.Now.AddHours(6);
                _userRepository.Update(user);
                await _unitOfWork.CommitAsync();
            }
            string callbackURL = Url.ActionLink("ForgetPassword", "Auth", new { token = user.ForgotPasswordToken, userID = user.ID }, Request.Scheme, Request.Host.Value );
            _ = Task.Run(async () => await _mail.SendMailAsync(user.Email, "Thay đổi mật khẩu", $"<a href={callbackURL}>Bấm vào đây</a>"));
            return Ok();
        }

        [HttpPost(("forget-password/callback"))]
        public async Task<IActionResult> ForgetPassword(string token, string userID, ForgetPasswordSubmit input)
        {
            User user = _userRepository.FindSingle(user => user.ID == userID);
            if (user == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Không tìm thấy user"
                });
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Token required"
                });
            }
            if (user.ForgotPasswordToken != null)
            {
                if (user.ForgotPasswordToken == token && user.ForgotPasswordTokenExpireAt >= DateTime.Now)
                {
                    user.Password = Encryptor.MD5Hash(input.Password);
                    user.ForgotPasswordToken = null;
                    user.ForgotPasswordTokenCreatedAt = null;
                    user.ForgotPasswordTokenExpireAt = null;
                    _userRepository.Update(user);
                    await _unitOfWork.CommitAsync();
                    return Ok(new
                    {
                        status = true,
                        message = "Đổi mật khẩu thành công"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Invalid Token"
                    });
                }
            }
            else
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid Action"
                });
            }    
        }
    }
}
