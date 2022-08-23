using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class UserRank
    {
        public int Rank { get; set; }
        public int ProblemSovled { get; set; }
        public UserDTO User { get; set; }
        public object Detail { get; set; }
    }
}
