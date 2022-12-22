using System;

namespace ICode.Web.Models.DTO
{
    public class StatisticResponse<T>
    {
        public int Total { get; set; }
        public DateTime Date { get; set; }
        public T Data { get; set; }
    }
}
