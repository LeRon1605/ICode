using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class Problem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }      
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ArticleID { get; set; }
        public virtual User Article { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<TestCase> TestCases { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
