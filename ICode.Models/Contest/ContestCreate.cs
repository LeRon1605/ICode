using ICode.Models.Validation_Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class ContestCreate
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int PlayerLimit { get; set; }
        [Required]
        public DateTime StartAt { get; set; }
        [Required]
        [DateGreaterThan("StartAt", "End date is not smaller than start date")]
        public DateTime EndAt { get; set; }
        [Required]
        [MinLength(1)]
        public ProblemContestCreate[] Problems { get; set; }
    }
}
