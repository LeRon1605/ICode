using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class ContestDetail
    {
        public string ContestID { get; set; }
        public string UserID { get; set; }
        public int Score { get; set; }
        public DateTime RegisteredAt { get; set; }
        public virtual Contest Contest { get; set; }
        public virtual User User { get; set; }
    }
}
