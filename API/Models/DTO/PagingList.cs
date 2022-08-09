using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.DTO
{
    public class PagingList<T> where T: class
    {
        public int TotalPage { get; set; }
        public int Page { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
