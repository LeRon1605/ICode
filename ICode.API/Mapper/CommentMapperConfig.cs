using AutoMapper;
using ICode.Data.Entity;
using ICode.Models.Comment;

namespace ICode.API.Mapper
{
    public class CommentMapperConfig : Profile
    {
        public CommentMapperConfig()
        {
            CreateMap<Comment, CommentBase>();
            CreateMap<Comment, CommentDetail>()
                    .IncludeBase<Comment, CommentBase>();
        }
    }
}
