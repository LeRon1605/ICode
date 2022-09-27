using AutoMapper;
using Data.Entity;
using Data.Repository;
using Data.Repository.Interfaces;
using Moq;
using Services;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Common;
using UnitTest.Data;
using Xunit;

namespace UnitTest.ServiceTest
{
    public class UserServiceTest
    {
        private readonly IUserService userService;

        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<IProblemRepository> problemRepositoryMock;
        private readonly Mock<ISubmissionRepository> submissionRepositoryMock;
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IRoleRepository> roleRepositoryMock;
        private readonly Mock<IMapper> mapperMock;

        private readonly List<User> users;

        public UserServiceTest()
        {
            users = UserData.GetUsers();

            userRepositoryMock = new Mock<IUserRepository>();
            problemRepositoryMock = new Mock<IProblemRepository>();
            submissionRepositoryMock = new Mock<ISubmissionRepository>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            roleRepositoryMock = new Mock<IRoleRepository>();   
            mapperMock = new Mock<IMapper>();

            #region Setup Mock
            // Setup mock for user Repository
            userRepositoryMock.Setup(x => x.FindAll()).Returns(UserData.GetUsers());
            userRepositoryMock.Setup(x => x.Update(It.IsAny<User>()));

            // Setup mock for unitOfWork
            unitOfWorkMock.Setup(x => x.CommitAsync());
            #endregion
            userService = new UserService(userRepositoryMock.Object, problemRepositoryMock.Object, roleRepositoryMock.Object, submissionRepositoryMock.Object, unitOfWorkMock.Object, mapperMock.Object);
        }

        [Fact]
        public void WhenGetAll_ThenShouldReturnList()
        {
            IEnumerable<User> users = userService.GetAll();

            Assert.NotNull(users);
            Assert.Equal(UserData.GetUsers().Count, users.Count());
            userRepositoryMock.Verify(x => x.FindAll(), Times.Once);
        }

        [Fact]
        public async Task GivenValidToken_WhenChangePassword_ThenShouldWork()
        {
            // Token not expired
            User user = new User
            {
                ID = UserConstant.ID,
                ForgotPasswordToken = TokenConstant.FORGET_PASSWORD_TOKEN,
                ForgotPasswordTokenExpireAt = DateTime.MaxValue
            };

            bool result = await userService.ChangePassword(user, TokenConstant.FORGET_PASSWORD_TOKEN, "New_Password");

            userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task GivenExpiredToken_WhenChangePassword_ThenShouldWork()
        {
            // Token expired
            User user = new User
            {
                ID = UserConstant.ID,
                ForgotPasswordToken = TokenConstant.FORGET_PASSWORD_TOKEN,
                ForgotPasswordTokenExpireAt = DateTime.MinValue
            };

            bool result = await userService.ChangePassword(user, TokenConstant.FORGET_PASSWORD_TOKEN, "New_Password");

            userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
            
            Assert.False(result);
        }
    }
}
