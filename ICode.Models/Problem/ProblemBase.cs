using System;
using ICode.Common;

namespace CodeStudy.Models
{
    public class ProblemBase
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
