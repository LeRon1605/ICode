﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStudy.Models
{
    public class ProblemDTO: ProblemBase
    {
        public int SuccessSubmit { get; set; }
        public int TotalSubmit { get; set; }
        public string Description { get; set; }
        public UserDTO Author { get; set; }
        public List<TagDTO> Tags { get; set; }
    }
}
