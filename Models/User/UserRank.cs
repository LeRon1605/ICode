using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class UserRank
    {
        public UserDTO User { get; set; }
        public int Rank { get; set; }
        public int ProblemSovled { get; set; }
        public int TotalSubmit { get; set; }
        public object Problems { get; set; }
    }
}
