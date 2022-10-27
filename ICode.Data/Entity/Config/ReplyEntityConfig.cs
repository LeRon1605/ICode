using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class ReplyEntityConfig : IEntityTypeConfiguration<Reply>
    {
        public void Configure(EntityTypeBuilder<Reply> builder)
        {
            builder.HasKey(reply => reply.ID);
            builder.Property(reply => reply.Content)
                  .IsRequired();
            builder.Property(reply => reply.CreatedAt)
                  .IsRequired();
            builder.Property(reply => reply.UpdatedAt)
                  .IsRequired(false)
                  .HasDefaultValue(null);
            builder.HasOne(reply => reply.Report)
                  .WithOne(report => report.Reply)
                  .HasForeignKey<Reply>(reply => reply.ID);
        }
    }
}
