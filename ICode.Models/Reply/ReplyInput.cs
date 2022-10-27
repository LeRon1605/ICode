using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeStudy.Models
{
    public class ReplyInput
    {
        [Required(ErrorMessage = "Nội dung phản hồi không được để trống")]
        [StringLength(256, MinimumLength = 10, ErrorMessage = "Nội dung phản hồi phải lớn hơn 10 kí tự")]
        public string Content { get; set; }
    }
}
