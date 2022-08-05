using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{
    public class ForgetPassword
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string Name { get; set; }
    }
}
