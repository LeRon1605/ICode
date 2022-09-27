using Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.Data
{
    public static class UserData
    {
        public static List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    ID = "1"
                },
                new User
                {
                    ID = "2"
                },
                new User
                {
                    ID = "3"
                }
            };
        }
    }
}
