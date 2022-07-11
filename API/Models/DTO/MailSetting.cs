using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.DTO
{
    public class MailSetting
    {
        public string FromEmailAddress { get; set; }
        public string FromEmailDisplayName { get; set; }
        public string FromEmailPassword { get; set; }
        public string SMTPHost { get; set; }
        public string SMTPPort { get; set; }
    }
}
