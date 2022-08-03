using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Token
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }
    public class AccessToken
    {
        public string ID { get; set; }
        public string Token { get; set; }
    }
}
