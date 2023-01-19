using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICode.Data.Entity.Config
{
    public class CommentEntityConfig : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(comment => comment.ID);
            builder.Property(comment => comment.Content)
                   .HasMaxLength(1024)
                   .IsRequired();
            builder.Property(comment => comment.At)
                   .IsRequired();
            builder.Property(comment => comment.UserID)
                   .IsRequired();
            builder.Property(comment => comment.ProblemID)
                   .IsRequired();
            builder.Property(comment => comment.ParentID)
                   .IsRequired(false);
            builder.HasOne(comment => comment.Problem)
                   .WithMany(problem => problem.Comments)
                   .HasForeignKey(comment => comment.ProblemID)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(comment => comment.User)
                   .WithMany(user => user.Comments)
                   .HasForeignKey(comment => comment.UserID)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(comment => comment.Parent)
                   .WithMany(comment => comment.Childs)
                   .HasForeignKey(comment => comment.ParentID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
