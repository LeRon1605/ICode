using System.ComponentModel.DataAnnotations;

namespace ICode.Models.Comment
{
    public class CommentUpdateDTO
    {
        [Required]
        [MinLength(10)]
        public string Content { get; set; }
    }
}
