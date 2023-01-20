using System.ComponentModel.DataAnnotations;

namespace ICode.Models.Comment
{
    public class CommentCreateDTO
    {
        [Required]
        [MinLength(10)]
        public string Content { get; set; }
        public string ParentId { get; set; }
    }
}
