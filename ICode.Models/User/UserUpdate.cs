using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{
    public class UserUpdate
    {
        public string UploadImage { get; set; }
        public bool? AllowNotification { get; set; }
    }
}
