using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.DTO
{
    public class ErrorResponse
    {
        public string error { get; set; }
        public object detail { get; set; }
    }
}
