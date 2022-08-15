using API.Helper;
using API.Models.Entity;
using API.Repository;
using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenProvider _tokenProvider;
        public TokenService(ITokenRepository tokenRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, TokenProvider tokenProvider)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _tokenProvider = tokenProvider;
        }

        public async Task<string> GenerateForgerPasswordToken(User user)
        {
            if (user.ForgotPasswordToken != null)
            {
                if (user.ForgotPasswordTokenExpireAt < DateTime.Now)
                {
                    user.ForgotPasswordToken = _tokenProvider.GenerateRandomToken();
                    user.ForgotPasswordTokenCreatedAt = DateTime.Now;
                    user.ForgotPasswordTokenExpireAt = DateTime.Now.AddHours(6);
                    _userRepository.Update(user);
                    await _unitOfWork.CommitAsync();
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
            return user.ForgotPasswordToken;
        }

        public async Task<Token> GenerateToken(User user)
        {
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
            return new Token
            {
                AccessToken = token.Token,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<Token> RefreshToken(string jwtID, string refresh_token)
        {
            RefreshToken refreshToken = _tokenRepository.FindSingle(x => x.Token == refresh_token);
            if (refreshToken == null || refreshToken.State || refreshToken.JwtID != jwtID)
            {
                return null;
            }
            User user = _userRepository.GetUserWithRole(user => user.ID == refreshToken.UserID);
            if (user == null)
            {
                return null;
            }
            refreshToken.State = true;
            Token newToken = await GenerateToken(user);
            return new Token
            {
                AccessToken = newToken.AccessToken,
                RefreshToken = newToken.RefreshToken
            };
        }

        public string ValidateToken(string accessToken)
        {
            string jwtId = null;
            _tokenProvider.ValidateToken(accessToken, ref jwtId);
            return jwtId;
        }
    }
}
