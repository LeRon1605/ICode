using ICode.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class ProblemContestCreate
    {
        [Required]
        public string ID { get; set; }
        [Required]
        public Level Level { get; set; }
        [Required]
        public int Score { get; set; }
    }
}
