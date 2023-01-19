namespace ICode.Models.Comment
{
    public class CommentBase
    {
        public string ID { get; set; }
        public string Content { get; set; }
        public string ParentID { get; set; }
        public string ProblemID { get; set; }
        public string UserID { get; set; }
    }
}
