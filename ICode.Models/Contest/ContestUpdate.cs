using ICode.Models.Validation_Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class ContestUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0, int.MaxValue)]
        public int? PlayerLimit { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public ProblemContestCreate[] Problems { get; set; }
    }
}
