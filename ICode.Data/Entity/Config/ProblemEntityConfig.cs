using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class ProblemEntityConfig : IEntityTypeConfiguration<Problem>
    {
        public void Configure(EntityTypeBuilder<Problem> builder)
        {
            builder.HasKey(problem => problem.ID);
            builder.Property(problem => problem.Name)
                  .IsRequired();
            builder.Property(problem => problem.Description)
                  .IsRequired();
            builder.Property(problem => problem.ArticleID)
                  .IsRequired();
            builder.Property(problem => problem.Status)
                  .IsRequired()
                  .HasDefaultValue(false);
            builder.Property(problem => problem.CreatedAt)
                  .IsRequired();
            builder.Property(problem => problem.UpdatedAt)
                  .IsRequired(false)
                  .HasDefaultValue(null);
            builder.HasOne(problem => problem.Article)
                  .WithMany(user => user.Problems)
                  .HasForeignKey(problem => problem.ArticleID);
            builder.HasMany(problem => problem.Reports)
                  .WithOne(report => report.Problem)
                  .HasForeignKey(report => report.ProblemID);
        }
    }
}
