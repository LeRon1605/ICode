using CodeStudy.Models;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface TokenProvider
    {
        AccessToken GenerateToken(User user);
        string GenerateRandomToken();
        bool ValidateToken(string token, ref string id);
    }
}
