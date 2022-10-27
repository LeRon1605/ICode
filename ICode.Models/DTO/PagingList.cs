using System.Collections.Generic;

namespace Models.DTO
{
    public class PagingList<T> where T: class
    {
        public int TotalPage { get; set; }
        public int Page { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
