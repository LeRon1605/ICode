﻿using API.Models.Entity;
using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ITokenService
    {
        Task<Token> GenerateToken(User user);
        Task<Token> RefreshToken(string jwtID, string refresh_token);
        string ValidateToken(string accessToken);
        Task<string> GenerateForgerPasswordToken(User user);
    }
}