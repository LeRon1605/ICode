﻿using Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class ProblemContestDetail
    {
        public string ContestID { get; set; }
        public string ProblemID { get; set; }
        public int Score { get; set; }
        public Level Level { get; set; }

        public virtual Problem Problem { get; set; }
        public virtual Contest Contest { get; set; }
    }
}
