using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ICode.Common;

namespace CodeStudy.Models
{
    public class ProblemInput
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(256, MinimumLength = 10, ErrorMessage = "Tên có độ dài từ 10 - 256 kí tự")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Mô tả không được để trống")]
        [MinLength(10)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Chứa ít nhất một testcase")]
        [MinLength(1, ErrorMessage = "Chứa ít nhất một testcase")]
        public List<TestcaseInput> TestCases { get; set; }
        [Required(ErrorMessage = "Chọn ít nhất một tag")]
        public List<string> Tags { get; set; }
        [Required]
        public Level Level { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Score { get; set; }
    }
}
