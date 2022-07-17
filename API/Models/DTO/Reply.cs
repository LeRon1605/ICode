using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.DTO
{
    public class ReplyInput
    {
        [Required]
        public string Content { get; set; }
    }
}
