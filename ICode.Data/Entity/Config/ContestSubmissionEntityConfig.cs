using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class ContestSubmissionEntityConfig : IEntityTypeConfiguration<ContestSubmission>
    {
        public void Configure(EntityTypeBuilder<ContestSubmission> builder)
        {
            builder.HasKey(x => x.SubmitID);

            builder.HasOne(submission => submission.Contest)
                   .WithMany(contest => contest.Submissions)
                   .HasForeignKey(submission => submission.ContestID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(contestSubmit => contestSubmit.Submission)
                   .WithOne(submission => submission.ContestSubmission)
                   .HasForeignKey<ContestSubmission>(contestSubmission => contestSubmission.SubmitID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
