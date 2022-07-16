using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.DTO
{
    public class ExecutorResult
    {
        public string output { get; set; }
        public int statusCode { get; set; }
        public string memory { get; set; }
        public string cpuTime { get; set; }
    }
}
