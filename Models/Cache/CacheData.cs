using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class CacheData
    {
        public string RecordID { get; set; }
        public object Data { get; set; }
        public DateTime CacheAt { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
