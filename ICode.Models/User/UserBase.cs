using System;
using System.Collections.Generic;
using System.Text;

namespace ICode.Models
{
    public class UserBase
    {
        public string ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string Gender { get; set; }
        public bool AllowNotification { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
