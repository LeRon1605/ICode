using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
    public class TestcaseInput
    {
        [Required]
        public string Input { get; set; }
        [Required]
        public string Output { get; set; }
        [Required]
        [Range(0, float.MaxValue)]
        public float TimeLimit { get; set; }
        [Required]
        [Range(0, float.MaxValue)]
        public float MemoryLimit { get; set; }
    }
}
