using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class RefreshToken
    {
        public string ID { get; set; }
        public string Token { get; set; }
        public string JwtID { get; set; }
        public bool State { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        public string UserID { get; set; }
        public virtual User User { get; set; }
    }
}
