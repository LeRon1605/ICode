using ICode.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{
    public class ProblemInputUpdate
    {
        [StringLength(256, MinimumLength = 10, ErrorMessage = "Tên có độ dài từ 10 - 256 kí tự")]
        public string Name { get; set; }
        [MinLength(10)]
        public string Description { get; set; }
        public bool? Status { get; set; }
        public string[] Tags { get; set; }
        public Level? Level { get; set; }
        public int? Score { get; set; }
    }
}
