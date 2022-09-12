using Castle.Core.Configuration;
using Data.Common;
using Data.Entity;
using Microsoft.Extensions.Configuration;
using Moq;
using Services;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTest.ServiceTest
{
    public class TokenProviderTest
    {
        private readonly TokenProvider tokenProvider;
        public TokenProviderTest()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                {"JWT:Key", "This is secret key of JWT token"},
            }).Build();

            tokenProvider = new JWTTokenProvider(configuration);
        }

        [Fact]
        public void GivenValidUser_WhenGenerateToken_ThenShouldWork()
        {
            User user = new User
            {
                ID = Guid.NewGuid().ToString(),
                Username = "Random Name",
                Role = new Role
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = Constant.USER
                }
            };

            var result = tokenProvider.GenerateToken(user);

            Assert.NotNull(result);
            Assert.NotEmpty(result.ID);
            Assert.NotEmpty(result.Token);
        }

        [Fact]
        public void GivenInvalidUser_WhenGenerateToken_ThenShouldNotWork()
        {
            User user = null;

            var result = tokenProvider.GenerateToken(user);

            Assert.Null(result);
        }

        [Fact]
        public void GivenValidToken_WhenValidateToken_ThenShouldWork()
        {
            User user = new User
            {
                ID = Guid.NewGuid().ToString(),
                Username = "Random Name",
                Role = new Role
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = Constant.USER
                }
            };

            var result = tokenProvider.GenerateToken(user);

            Assert.NotNull(result);
            Assert.NotEmpty(result.ID);
            Assert.NotEmpty(result.Token);

            string tokenID = null;
            var validateResult = tokenProvider.ValidateToken(result.Token, ref tokenID);
            Assert.True(validateResult);
            Assert.NotNull(tokenID);
            Assert.Equal(result.ID, tokenID);
        }


        [Fact]
        public void GivenInValidToken_WhenValidateToken_ThenShouldNotWork()
        {
            string tokenID = null;
            var validateResult = tokenProvider.ValidateToken("", ref tokenID);
            Assert.False(validateResult);
            Assert.Null(tokenID);
        }
    }
}
