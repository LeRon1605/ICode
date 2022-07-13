using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Entity
{
    public class Role
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }

    public class RoleUpdate
    {
        [Required]
        public string Name { get; set; }
    }
}
