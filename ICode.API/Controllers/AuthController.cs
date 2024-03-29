﻿using API.Filter;
using CodeStudy.Models;
using Data.Entity;
using Google.Apis.Auth;
using ICode.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using Newtonsoft.Json;
using Services;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userSerivce;
        private readonly IRoleService _roleService;
        private readonly ITokenService _tokenService;
        private readonly IMailService _mail;
        
        public AuthController(IUserService userService, IRoleService roleService, ITokenService tokenSerivce, IMailService mail)
        {
            _userSerivce = userService;
            _roleService = roleService;
            _tokenService = tokenSerivce;
            _mail = mail;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Auth()
        {
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUser input)
        {
            if (_userSerivce.Exist(input.Username, input.Email))
            {
                return Conflict(new ErrorResponse
                {
                    error = "Register failed.",
                    detail = "Username or email already exist."
                });
            }
            else
            {
                await _userSerivce.Add(new User
                {
                    ID = Guid.NewGuid().ToString(),
                    Email = input.Email,
                    Password = Encryptor.MD5Hash(input.Password),
                    Username = input.Username,
                    Avatar = input.Gender ? Constant.MALE_AVT : Constant.FEMALE_AVT,
                    Gender = input.Gender,
                    CreatedAt = DateTime.Now,
                    Type = AccountType.Local,
                    RoleID = _roleService.FindByName(Constant.USER).ID,
                    AllowNotification = input.AllowNotification ?? false
                });
                return Ok();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUser input, [FromServices] ILocalAuth authHandler)
        {
            User user = await _userSerivce.Login(input.Name, input.Password, authHandler);
            if (user == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Login failed.",
                    detail = "User doest not exist."
                });
            }
            Token token = await _tokenService.GenerateToken(user);
            return Ok(new 
            {
                access_token = token.AccessToken,
                refresh_token = token.RefreshToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh(Token token)
        {
            string jwtId = _tokenService.ValidateToken(token.AccessToken);
            if (jwtId == null)
            {
                return BadRequest(new ErrorResponse 
                { 
                    error = "Invalid token.",
                    detail = "Validated acccess_token error."
                });
            }

            Token newToken = await _tokenService.RefreshToken(jwtId, token.RefreshToken);
            if (newToken == null)
            {
                return BadRequest(new ErrorResponse 
                { 
                    error = "Invalid token.",
                    detail = "Refresh token has been used or doesn't exist."
                });
            }
            else
            {
                return Ok(new
                {
                    access_token = newToken.AccessToken,
                    refresh_token = newToken.RefreshToken
                });
            }
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPassword input)
        {
            User user = _userSerivce.FindByName(input.Name);
            if (user == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "User doesn't exist."
                });
            }
            string forgetPasswordToken = await _tokenService.GenerateForgerPasswordToken(user);
            string callbackURL = Url.ActionLink("ForgetPassword", "Auth", new { token = forgetPasswordToken, userID = user.ID }, "http", "icode1605.tech");
            _ = Task.Run(async () => await _mail.SendMailAsync(user.Email, "Thay đổi mật khẩu", $"<a href={callbackURL}>Bấm vào đây</a>"));
            return Ok();
        }

        [HttpPost(("forget-password/callback"))]
        [QueryConstraint(Key = "token")]
        public async Task<IActionResult> ForgetPassword(string token, string userID, ForgetPasswordSubmit input)
        {
            User user = _userSerivce.FindByID(userID);
            if (user == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "User doesn't exist."
                });
            }
            if (await _userSerivce.ChangePassword(user, token, input.Password))
            {
                return Ok();
            }
            return BadRequest(new ErrorResponse
            {
                error = "Invalid token.",
                detail = "Token incorrect or already been used."
            });
        }

        [HttpPost("google-signin")]
        public async Task<IActionResult> SignInWithGoogle(Token access_token, [FromServices] IGoogleAuth authHandler)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={access_token.AccessToken}");
            if (response.IsSuccessStatusCode)
            {
                GooglePayload payload = JsonConvert.DeserializeObject<GooglePayload>(await response.Content.ReadAsStringAsync());
                User user = await _userSerivce.Login(payload.email, Constant.PASSWORD_DEFAULT, authHandler);
                if (user == null)
                {
                    user = new User
                    {
                        ID = Guid.NewGuid().ToString(),
                        Email = payload.email,
                        Password = Encryptor.MD5Hash(Constant.PASSWORD_DEFAULT),
                        Avatar = payload.picture,
                        Username = payload.name,
                        CreatedAt = DateTime.Now,
                        Type = AccountType.Google,
                        AllowNotification = true,
                        Gender = true,
                        RoleID = _roleService.FindByName(Constant.USER).ID
                    };
                    await _userSerivce.Add(user);
                }
                Token token = await _tokenService.GenerateToken(user);
                return Ok(new
                {
                    access_token = token.AccessToken,
                    refresh_token = token.RefreshToken
                });
            }
            return BadRequest(new {
                message = "Invalid token"
            });
        }
    }
}
