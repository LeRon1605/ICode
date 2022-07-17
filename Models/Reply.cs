using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeStudy.Models
{
    public class ReplyInput
    {
        [Required]
        public string Content { get; set; }
    }
}
