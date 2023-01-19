using CodeStudy.Models;
using System.Collections.Generic;

namespace ICode.Models.Comment
{
    public class CommentDetail: CommentBase
    {
        public UserDTO User { get; set; }
        public List<CommentDetail> Childs { get; set; }
    }
}
