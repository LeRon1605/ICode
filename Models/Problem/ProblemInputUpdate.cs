using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStudy.Models
{
    public class ProblemInputUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public string[] Tags { get; set; }
    }
}
