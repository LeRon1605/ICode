namespace ICode.Web.Models.DTO
{
    public class ServiceResponse<T>
    {
        public bool State { get; set; }
        public T Data { get; set; }
        public string Description { get; set; }
    }
}
