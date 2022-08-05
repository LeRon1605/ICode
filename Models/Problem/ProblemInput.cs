using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{
    public class ProblemInput
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(256, MinimumLength = 10, ErrorMessage = "Tên có độ dài từ 10 - 256 kí tự")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Mô tả không được để trống")]
        [StringLength(256, MinimumLength = 10, ErrorMessage = "Mô tả có độ dài từ 10 - 256 kí tự")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Chứa ít nhất một testcase")]
        [MinLength(1, ErrorMessage = "Chứa ít nhất một testcase")]
        public List<TestcaseInput> TestCases { get; set; }
        public List<string> Tags { get; set; }
    }
}
