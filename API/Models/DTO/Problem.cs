using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.DTO
{
    public class ProblemDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public UserDTO Article { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<TagDTO> Tags { get; set; }
    }    
    public class ProblemInput
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ArticleID { get; set; }
        [Required]
        public List<TestcaseInput> TestCases { get; set; }
        public string[] Tags { get; set; }
    }

    public class ProblemInputUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public string[] Tags { get; set; }
    }
}
