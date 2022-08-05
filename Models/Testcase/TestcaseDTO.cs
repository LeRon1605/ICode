using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStudy.Models
{
    public class TestcaseDTO
    {
        public string ID { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public float TimeLimit { get; set; }
        public float MemoryLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
