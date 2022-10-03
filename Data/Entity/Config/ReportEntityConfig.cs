using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class ReportEntityConfig : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.HasKey(report => report.ID);
            builder.Property(report => report.Title)
                  .HasMaxLength(255)
                  .IsRequired();
            builder.Property(report => report.Content)
                  .IsRequired();
            builder.Property(report => report.CreatedAt)
                  .IsRequired();
            builder.Property(report => report.UpdatedAt)
                  .IsRequired(false)
                  .HasDefaultValue(null);
            builder.Property(report => report.UserID)
                  .IsRequired();
            builder.Property(report => report.ProblemID)
                  .IsRequired();
            builder.HasOne(report => report.User)
                  .WithMany(user => user.Reports)
                  .HasForeignKey(report => report.UserID)
                  .OnDelete(DeleteBehavior.NoAction);
            //entity.HasOne(report => report.Problem)
            //      .WithMany(problem => problem.Reports)
            //      .HasForeignKey(report => report.ProblemID);
        }
    }
}
