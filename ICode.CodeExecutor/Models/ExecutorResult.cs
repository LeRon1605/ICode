namespace ICode.CodeExecutor.Models
{
    public class ExecutorResult
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public float Time { get; set; }
        public int Memory { get; set; }
        public ExecutorStatus Status { get; set; }
    }
}
