using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class LoginResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string token { get; set; }
    }
}
