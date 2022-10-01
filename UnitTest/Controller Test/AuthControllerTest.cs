using API.Controllers;
using CodeStudy.Models;
using Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Moq;
using Services;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.ControllerTest
{
    public class AuthControllerTest
    {
        private readonly AuthController authController;

        private readonly Mock<IUserService> userSerivceMock;
        private readonly Mock<IRoleService> roleServiceMock;
        private readonly Mock<ITokenService> tokenServiceMock;
        private readonly Mock<IMailService> mailMock;
        public AuthControllerTest()
        {
            userSerivceMock = new Mock<IUserService>();
            roleServiceMock = new Mock<IRoleService>();
            tokenServiceMock = new Mock<ITokenService>();
            mailMock = new Mock<IMailService>();

            authController = new AuthController(userSerivceMock.Object, roleServiceMock.Object, tokenServiceMock.Object, mailMock.Object);
        }

        [Fact]
        public async Task GivenExistUser_WhenRegister_ThenShouldReturnConflict()
        {
            // Already exist
            userSerivceMock.Setup(x => x.Exist(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            IActionResult result = await authController.Register(new RegisterUser
            {
                Email = "Email",
                Password = "Password"
            });

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var returnObj = Assert.IsType<ErrorResponse>(conflictResult.Value);
        }

        [Fact]
        public async Task GivenValidUser_WhenRegister_ThenShouldWork()
        {
            userSerivceMock.Setup(x => x.Exist(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            roleServiceMock.Setup(x => x.FindByName("User")).Returns(new RoleDTO
            {
                ID = Guid.NewGuid().ToString(),
            });

            IActionResult result = await authController.Register(new RegisterUser
            {
                Email = "Email",
                Password = "Password",
                Username = "Username",
                Gender = true,
            });

            var okResult = Assert.IsType<OkResult>(result);
            userSerivceMock.Verify(x => x.Add(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task GivenInvalidUser_WhenLogin_ThenShouldReturnNotFound()
        {
            userSerivceMock.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ILocalAuth>())).Returns(value: null);
            IActionResult result = await authController.Login(new LoginUser
            {
                Name = "Name",
                Password = "Password"
            }, new LocalAuth());

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnObj = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        }

        [Fact]
        public async Task GivenValidUser_WhenLogin_ThenShouldReturnToken()
        {
            userSerivceMock.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ILocalAuth>())).Returns(Task.FromResult(new User
            {
                ID = Guid.NewGuid().ToString()
            }));
            tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns(Task.FromResult(new Token
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token"
            }));

            IActionResult result = await authController.Login(new LoginUser
            {
                Name = "Name",
                Password = "Password"
            }, new LocalAuth());

            var okResult = Assert.IsType<OkObjectResult>(result);
            string access_token = (string)okResult.Value.GetType().GetProperty("access_token").GetValue(okResult.Value, null);
            string refresh_token = (string)okResult.Value.GetType().GetProperty("refresh_token").GetValue(okResult.Value, null);
            Assert.Equal("access_token", access_token);
            Assert.Equal("refresh_token", refresh_token);
            tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task GivenInvalidAccessToken_WhenRefreshToken_ThenShouldReturnBadRequest()
        {
            tokenServiceMock.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(value: null);
            IActionResult result = await authController.Refresh(new Token
            {
                AccessToken = "invalid_access_token",
                RefreshToken = "refresh_token"
            });

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnObj = Assert.IsType<ErrorResponse>(badResult.Value);
        }

        [Fact]
        public async Task GivenInValidRefreshToken_WhenRefreshToken_ThenShouldReturnBadRequest()
        {
            tokenServiceMock.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns("jwtid");
            tokenServiceMock.Setup(x => x.RefreshToken(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Token)null));
            
            IActionResult result = await authController.Refresh(new Token
            {
                AccessToken = "access_token",
                RefreshToken = "invalid_refresh_token"
            });

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnObj = Assert.IsType<ErrorResponse>(badResult.Value);
        }

        [Fact]
        public async Task GivenValidToken_WhenRefreshToken_ThenShouldReturnOk()
        {
            tokenServiceMock.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns("jwtid");
            tokenServiceMock.Setup(x => x.RefreshToken(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new Token
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token"
            }));

            IActionResult result = await authController.Refresh(new Token
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token"
            });

            var okResult = Assert.IsType<OkObjectResult>(result);
            string access_token = (string)okResult.Value.GetType().GetProperty("access_token").GetValue(okResult.Value, null);
            string refresh_token = (string)okResult.Value.GetType().GetProperty("refresh_token").GetValue(okResult.Value, null);
            Assert.Equal("access_token", access_token);
            Assert.Equal("refresh_token", refresh_token);
        }

        [Fact]
        public async Task GivenInvalidUsername_WhenGetForgetPasswordToken_ThenShouldReturnNotFound()
        {
            userSerivceMock.Setup(x => x.FindByName(It.IsAny<string>())).Returns(value: null);

            IActionResult result = await authController.ForgetPassword(new ForgetPassword
            {
                Name = "invalid_user"
            });

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnObj = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        }

        [Fact]
        public async Task GivenValidUsername_WhenGetForgetPasswordToken_ThenShouldReturnOkAndSendMail()
        {
            userSerivceMock.Setup(x => x.FindByName(It.IsAny<string>())).Returns(new User
            {
                ID = Guid.NewGuid().ToString(),
                Email = "user_email"
            });
            tokenServiceMock.Setup(x => x.GenerateForgerPasswordToken(It.IsAny<User>())).Returns(Task.FromResult("forget_password_token"));

            authController.Request.Host = new HostString("localhost");
            authController.Request.Scheme = "http";
            IActionResult result = await authController.ForgetPassword(new ForgetPassword
            {
                Name = "user"
            });

            var okResult = Assert.IsType<OkResult>(result);
            mailMock.Verify(x => x.SendMailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

    }


}
