using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{
    public class SubmissionInput
    {
        [Required(ErrorMessage = "Mã code không được để trống")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Ngôn ngữ không được để trống")]
        public string Language { get; set; }
    }
}
