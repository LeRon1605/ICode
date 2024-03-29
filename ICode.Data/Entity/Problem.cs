﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICode.Common;
using ICode.Data.Entity;

namespace Data.Entity
{
    public class Problem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }      
        public bool Status { get; set; }
        public int Score { get; set; }
        public Level Level { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ArticleID { get; set; }
        public virtual User Article { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<TestCase> TestCases { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<ProblemContestDetail> ProblemContestDetails { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
