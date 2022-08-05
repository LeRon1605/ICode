using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{
    public class ReportInput
    {
        [Required(ErrorMessage = "Tiêu đề bình luận không được để trống")]
        [StringLength(256, MinimumLength = 10, ErrorMessage = "Tiêu đề có độ dài tối thiểu 10 kí tự")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Nội dung bình luận không được để trống")]
        [StringLength(256, MinimumLength = 10, ErrorMessage = "Nội dung bình luận có độ dài tối thiểu 10 kí tự")]
        public string Content { get; set; }
    }
}
