using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{
    public class ForgetPasswordSubmit
    {
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Mật khẩu có độ dài tối thiểu 8 kí tự")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Mật khẩu xác nhận không được để trống")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không chính xác")]
        public string ConfirmPassword { get; set; }
    }
}
