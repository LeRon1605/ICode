using API.Models.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Helper
{
    public interface TokenProvider
    {
        string GenerateToken(User user);
    }
    public class JWTTokenProvider: TokenProvider
    {
        public IConfiguration _configuration;
        public JWTTokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: new Claim[]
                {
                    new Claim("ID", user.ID),
                    new Claim("Role", user.Role.Name)
                },
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
