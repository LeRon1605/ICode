using Data.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ICode.Data.Entity
{
    public class Comment
    {
        public string ID { get; set; }
        public string Content { get; set; }
        public DateTime At { get; set; }
        public string ProblemID { get; set; }
        public string UserID { get; set; }
        public string ParentID { get; set; }
        public virtual Problem Problem { get; set; }
        public virtual User User { get; set; }
        public virtual Comment Parent { get; set; }
        public virtual ICollection<Comment> Childs { get; set; }
    }
}
