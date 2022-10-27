using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class SubmissionDetailEntityConfig : IEntityTypeConfiguration<SubmissionDetail>
    {
        public void Configure(EntityTypeBuilder<SubmissionDetail> builder)
        {
            builder.HasKey(detail => new { detail.SubmitID, detail.TestCaseID });
            builder.Property(detail => detail.State)
                  .IsRequired();
            builder.Property(detail => detail.Memory)
                  .IsRequired();
            builder.Property(detail => detail.Time)
                  .IsRequired();
            builder.HasOne(detail => detail.Submission)
                  .WithMany(detail => detail.SubmissionDetails)
                  .HasForeignKey(detail => detail.SubmitID)
                  .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(detail => detail.TestCase)
                  .WithMany(testcase => testcase.SubmissionDetails)
                  .HasForeignKey(detail => detail.TestCaseID)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
