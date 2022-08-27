using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class Report
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserID { get; set; }
        public string ProblemID { get; set; }
        public virtual User User { get; set; }
        public virtual Reply Reply { get; set; }
        public virtual Problem Problem { get; set; }
    }
}
