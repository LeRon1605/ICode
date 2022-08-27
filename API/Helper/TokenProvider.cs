using CodeStudy.Models;
using Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Helper
{
    public interface TokenProvider
    {
        AccessToken GenerateToken(User user);
        string GenerateRandomToken();
        bool ValidateToken(string token, ref string id);
    }
    public class JWTTokenProvider: TokenProvider
    {
        public IConfiguration _configuration;
        public JWTTokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AccessToken GenerateToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: new Claim[]
                {
                    new Claim(Constant.ID, user.ID),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(Constant.ROLE, user.Role.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                },
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );
            return new AccessToken
            {
                ID = token.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

        public string GenerateRandomToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public bool ValidateToken(string token, ref string id)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var tokenValidationParam = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero,
                };
                var tokenInVerification = jwtTokenHandler.ValidateToken(token, tokenValidationParam, out var validatedToken);
                
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    id = jwtSecurityToken.Id;
                    // So sánh thuật toán
                    if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256)) 
                    {
                        return false;
                    }
                }
                
                // Check if token is expire
                if (validatedToken.ValidTo > DateTime.Now)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
