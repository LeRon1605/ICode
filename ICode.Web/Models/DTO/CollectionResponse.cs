using System;

namespace ICode.Web.Models.DTO
{
    public class CollectionResponse<T>
    {
        public string RecordID { get; set; }
        public T Data { get; set; }
        public DateTime CacheAt { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
