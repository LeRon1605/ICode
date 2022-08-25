using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{
    public class UserUpdate
    {
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Username có độ dài từ 6 - 16 kí tự")]
        [Required(ErrorMessage = "Username không được để trống")]
        public string Username { get; set; }
        public string UploadImage { get; set; }
    }
}
