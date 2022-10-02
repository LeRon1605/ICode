using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class ContestSubmissionConfig : IEntityTypeConfiguration<ContestSubmission>
    {
        public void Configure(EntityTypeBuilder<ContestSubmission> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.Code)
                   .IsRequired();
            builder.Property(x => x.Language)
                   .IsRequired();
            builder.Property(x => x.CreatedAt)
                   .IsRequired();
            
            builder.HasOne(submission => submission.User)
                   .WithMany(user => user.ContestSubmissions)
                   .HasForeignKey(submission => submission.UserID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(submission => submission.Contest)
                   .WithMany(contest => contest.Submissions)
                   .HasForeignKey(submission => submission.Contest);
        }
    }
}
