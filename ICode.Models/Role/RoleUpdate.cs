using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeStudy.Models
{

    public class RoleUpdate
    {
        [Required]
        public string Name { get; set; }
    }
}
