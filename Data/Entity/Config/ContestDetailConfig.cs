using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class ContestDetailConfig : IEntityTypeConfiguration<ContestDetail>
    {
        public void Configure(EntityTypeBuilder<ContestDetail> builder)
        {
            builder.HasKey(x => new { x.UserID, x.ContestID });
            builder.Property(x => x.Score)
                   .IsRequired()
                   .HasDefaultValue(0);
            builder.Property(x => x.RegisteredAt)
                   .IsRequired();

            builder.HasOne(detail => detail.User)
                   .WithMany(user => user.ContestDetails)
                   .HasForeignKey(detail => detail.UserID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(detail => detail.Contest)
                   .WithMany(contest => contest.ContestDetails)
                   .HasForeignKey(detail => detail.ContestID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
