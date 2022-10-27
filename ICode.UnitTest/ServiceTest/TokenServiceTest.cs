using API.Services;
using CodeStudy.Models;
using Data.Common;
using Data.Entity;
using Data.Repository;
using Data.Repository.Interfaces;
using Moq;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Common;
using Xunit;

namespace UnitTest.ServiceTest
{
    public class TokenServiceTest
    {
        private readonly ITokenService tokenService;
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<ITokenRepository> tokenRepositoryMock;
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<TokenProvider> tokenProviderMock;

        public TokenServiceTest()
        {
            tokenRepositoryMock = new Mock<ITokenRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            tokenProviderMock = new Mock<TokenProvider>();

            #region Set up mock
            // Setup userRepository Mock
            userRepositoryMock.Setup(x => x.GetUserWithRole(It.IsAny<Expression<Func<User, bool>>>())).Returns(new User
            {
                ID = UserConstant.ID,
                Role = new Role
                {
                    Name = Constant.USER
                }
            });
            userRepositoryMock.Setup(x => x.Update(It.IsAny<User>()));
            userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>()));

            // Setup tokenRepository Mock
            tokenRepositoryMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>()));
            
            // Setup unitOfWork Mock
            unitOfWorkMock.Setup(x => x.CommitAsync());

            // Setup tokenProviderMock
            tokenProviderMock.Setup(x => x.GenerateRandomToken())
                             .Returns(TokenConstant.RANDOM_TOKEN);
            tokenProviderMock.Setup(x => x.GenerateToken(It.IsAny<User>()))
                             .Returns(new AccessToken
                             {
                                 ID = TokenConstant.ID,
                                 Token = TokenConstant.ACCESS_TOKEN
                             });
            #endregion

            tokenService = new TokenService(tokenRepositoryMock.Object, userRepositoryMock.Object, unitOfWorkMock.Object, tokenProviderMock.Object);    
        }

        [Fact]
        public async Task GivenExpiredToken_WhenGenerateForgetPassWordToken_ThenShouldGiveNewToken()
        {
            string expected = TokenConstant.RANDOM_TOKEN;

            User user = new User
            {
                ID = Guid.NewGuid().ToString(),
                ForgotPasswordToken = TokenConstant.OLD_FORGET_PASSWORD_TOKEN,
                ForgotPasswordTokenExpireAt = DateTime.MinValue
            };

            string token = await tokenService.GenerateForgerPasswordToken(user);

            userRepositoryMock.Verify(x => x.Update(user), Times.Once());
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once());

            Assert.NotEmpty(token);
            Assert.Equal(token, expected);
            Assert.NotEqual(token, TokenConstant.OLD_FORGET_PASSWORD_TOKEN);
            Assert.True(user.ForgotPasswordTokenExpireAt > DateTime.Now);
        }

        [Fact]
        public async Task GivenValidToken_WhenGenerateForgetPassWordToken_ThenShouldGiveBackToken()
        {
            string expected = TokenConstant.FORGET_PASSWORD_TOKEN;
            User user = new User
            {
                ID = Guid.NewGuid().ToString(),
                ForgotPasswordToken = TokenConstant.FORGET_PASSWORD_TOKEN,
                ForgotPasswordTokenExpireAt = DateTime.Now.AddDays(1)
            };

            string token = await tokenService.GenerateForgerPasswordToken(user);

            Assert.NotEmpty(token);
            Assert.Equal(token, expected);
            Assert.True(((DateTime)user.ForgotPasswordTokenExpireAt).Date == DateTime.Now.AddDays(1).Date);
        }

        [Fact]
        public async Task GivenNoToken_WhenGenerateForgetPassWordToken_ThenShouldGiveNewToken()
        {
            string expected = TokenConstant.RANDOM_TOKEN;

            User user = new User
            {
                ID = Guid.NewGuid().ToString(),
                ForgotPasswordToken = null
            };

            string token = await tokenService.GenerateForgerPasswordToken(user);

            userRepositoryMock.Verify(x => x.Update(user), Times.Once());
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once());

            Assert.NotEmpty(token);
            Assert.Equal(token, expected);
            Assert.True(user.ForgotPasswordTokenExpireAt > DateTime.Now);
        }

        [Fact]
        public async Task GivenValidUser_WhenGenerateToken_ThenShouldGivenToken()
        {
            Token expected = new Token
            {
                AccessToken = TokenConstant.ACCESS_TOKEN,
                RefreshToken = TokenConstant.RANDOM_TOKEN
            };
            User user = new User();

            Token token = await tokenService.GenerateToken(user);

            tokenProviderMock.Verify(x => x.GenerateToken(user), Times.Once());
            tokenProviderMock.Verify(x => x.GenerateRandomToken(), Times.Once());

            tokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once());

            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once());

            Assert.NotNull(token);
            Assert.Equal(token.AccessToken, expected.AccessToken);
            Assert.Equal(token.RefreshToken, expected.RefreshToken);
        }

        [Fact]
        public async Task GivenInvalidUser_WhenGenerateToken_ThenShouldNotWork()
        {
            Token token = await tokenService.GenerateToken(null);

            Assert.Null(token);
        }

        [Fact]
        public async Task GivenValidRefreshToken_WhenGenerateRefreshToken_ThenShouldWork()
        {
            // Mock for refresh token has not been used and has not been expired yet
            tokenRepositoryMock.Setup(x => x.FindSingle(It.IsAny<Expression<Func<RefreshToken, bool>>>())).Returns(new RefreshToken
            {
                State = false,
                JwtID = TokenConstant.ID,
                ExpiredAt = DateTime.MaxValue
            });

            Token token = await tokenService.RefreshToken(TokenConstant.ID, TokenConstant.REFRESH_TOKEN);

            tokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Exactly(2));
            Assert.NotNull(token);
        }

        [Fact]
        public async Task GivenExpiredRefreshToken_WhenGenerateRefreshToken_ThenShouldReturnNull()
        {
            // Mock for refresh token has not been used and has already been expired
            tokenRepositoryMock.Setup(x => x.FindSingle(It.IsAny<Expression<Func<RefreshToken, bool>>>())).Returns(new RefreshToken
            {
                State = false,
                JwtID = TokenConstant.ID,
                ExpiredAt = DateTime.MinValue
            });

            Token token = await tokenService.RefreshToken(TokenConstant.ID, TokenConstant.REFRESH_TOKEN);
            
            // Invalid token never affect to db
            tokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Never);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);

            Assert.Null(token);
        }

        [Fact]
        public async Task GivenUsedRefreshToken_WhenGenerateRefreshToken_ThenShouldReturnNull()
        {
            // Mock for refresh token has already been used and has not been expired yet
            tokenRepositoryMock.Setup(x => x.FindSingle(It.IsAny<Expression<Func<RefreshToken, bool>>>())).Returns(new RefreshToken
            {
                State = true,
                JwtID = TokenConstant.ID,
                ExpiredAt = DateTime.MaxValue
            });

            Token token = await tokenService.RefreshToken(TokenConstant.ID, TokenConstant.REFRESH_TOKEN);

            // Invalid token never affect to db
            tokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Never);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);

            Assert.Null(token);
        }
    }
}
