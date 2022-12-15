using System;
using System.Collections.Generic;
using System.Text;

namespace ICode.Models
{
    public class UserContest
    {
        public UserBase User { get; set; }
        public int Score { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
