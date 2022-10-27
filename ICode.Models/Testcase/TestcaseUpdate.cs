using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{
    public class TestcaseUpdate
    {

        public string Input { get; set; }
        public string Output { get; set; }
        [Range(0, float.MaxValue, ErrorMessage = "Time Limit không được bé hơn 0")]
        public float? TimeLimit { get; set; }
        [Range(0, float.MaxValue, ErrorMessage = "Memory Limit không được bé hơn 0")]
        public float? MemoryLimit { get; set; }
    }
}
