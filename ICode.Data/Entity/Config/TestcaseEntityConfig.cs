using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class TestcaseEntityConfig : IEntityTypeConfiguration<TestCase>
    {
        public void Configure(EntityTypeBuilder<TestCase> builder)
        {
            builder.HasKey(testcase => testcase.ID);
            builder.Property(testcase => testcase.Input)
                  .IsRequired();
            builder.Property(testcase => testcase.Output)
                  .IsRequired();
            builder.Property(testcase => testcase.MemoryLimit)
                  .HasColumnType("float")
                  .IsRequired();
            builder.Property(testcase => testcase.TimeLimit)
                  .HasColumnType("float")
                  .IsRequired();
            builder.Property(testcase => testcase.CreatedAt)
                  .IsRequired();
            builder.Property(testcase => testcase.UpdatedAt)
                  .IsRequired(false)
                  .HasDefaultValue(null);
            builder.Property(testcase => testcase.ProblemID)
                  .IsRequired();
            builder.HasOne(testcase => testcase.Problem)
                  .WithMany(problem => problem.TestCases)
                  .HasForeignKey(testcase => testcase.ProblemID)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
