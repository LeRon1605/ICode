using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{
    public class TagInput
    {
        [Required(ErrorMessage = "Tên tag không được để trống")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Tên tag có độ dài từ 5 - 30 kí tự")]
        public string Name { get; set; }
    }
}
