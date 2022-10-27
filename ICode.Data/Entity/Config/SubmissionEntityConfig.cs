using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class SubmissionEntityConfig : IEntityTypeConfiguration<Submission>
    {
        public void Configure(EntityTypeBuilder<Submission> builder)
        {
            builder.HasKey(submit => submit.ID);
            builder.Property(submit => submit.Code)
                  .IsRequired();
            builder.Property(submit => submit.Language)
                  .IsRequired();
            builder.Property(submit => submit.CreatedAt)
                  .IsRequired();
            builder.Property(submit => submit.State)
                  .IsRequired();
            builder.Property(submit => submit.UserID)
                  .IsRequired();
            builder.HasOne(submit => submit.User)
                  .WithMany(user => user.Submissions)
                  .HasForeignKey(submit => submit.UserID)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
