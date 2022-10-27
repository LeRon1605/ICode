using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{
    public class RegisterUser
    {
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Username có độ dài từ 6 - 16 kí tự")]
        [Required(ErrorMessage = "Username không được để trống")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Mật khẩu có độ dài tối thiểu 8 kí tự")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Mật khẩu xác nhận không được để trống")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không chính xác")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Giới tính không được để trống")]
        public bool Gender { get; set; }
        public bool? AllowNotification { get; set; }
    }
}
