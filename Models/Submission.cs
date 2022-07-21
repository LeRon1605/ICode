using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeStudy.Models
{
    public class SubmissionDTO
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Language { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserID { get; set; }
    }
    public class SubmissionInput
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Language { get; set; }
    }
}
