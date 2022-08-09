using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStudy.Models
{
    public class ProblemDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public UserDTO Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<TagDTO> Tags { get; set; }
    }
}
