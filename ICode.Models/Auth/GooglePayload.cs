using System;
using System.Collections.Generic;
using System.Text;

namespace Models 
{ 
    public class GooglePayload
    {
        public string sub { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
        public string email { get; set; }
    }
}
