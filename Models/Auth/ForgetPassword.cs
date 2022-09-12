using System.ComponentModel.DataAnnotations;

namespace CodeStudy.Models
{
    public class ForgetPassword
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string Name { get; set; }
    }
}
