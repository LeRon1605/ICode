using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.DTO
{
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
