using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ContestBase
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PlayerLimit { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
