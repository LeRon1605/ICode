using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Entity
{
    public class User
    {
        public string ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string RoleID { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
        public virtual ICollection<Problem> Problems { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
