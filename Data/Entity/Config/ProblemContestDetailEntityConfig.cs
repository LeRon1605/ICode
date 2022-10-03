using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class ProblemContestDetailEntityConfig : IEntityTypeConfiguration<ProblemContestDetail>
    {
        public void Configure(EntityTypeBuilder<ProblemContestDetail> builder)
        {
            builder.HasKey(x => new { x.ContestID, x.ProblemID });
            builder.Property(x => x.Score)
                   .IsRequired();
            builder.Property(x => x.Level)
                   .IsRequired();

            builder.HasOne(detail => detail.Problem)
                   .WithMany(problem => problem.ProblemContestDetails)
                   .HasForeignKey(detail => detail.ProblemID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(detail => detail.Contest)
                   .WithMany(contest => contest.ProblemContestDetails)
                   .HasForeignKey(detail => detail.ContestID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
