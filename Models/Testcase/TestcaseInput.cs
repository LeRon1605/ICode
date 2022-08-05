using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{
    public class TestcaseInput
    {
        [Required(ErrorMessage = "Input cho testcase không được để trống")]
        public string Input { get; set; }
        [Required(ErrorMessage = "Output cho testcase không được để trống")]
        public string Output { get; set; }
        [Required(ErrorMessage = "Time Limit cho testcase không được để trống")]
        [Range(0, float.MaxValue, ErrorMessage = "Time Limit không được bé hơn 0")]
        public float TimeLimit { get; set; }
        [Required(ErrorMessage = "Memory Limit cho testcase không được để trống")]
        [Range(0, float.MaxValue, ErrorMessage = "Time Limit không được bé hơn 0")]
        public float MemoryLimit { get; set; }
    }
}
