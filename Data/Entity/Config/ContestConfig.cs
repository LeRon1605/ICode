using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class ContestConfig: IEntityTypeConfiguration<Contest>
    {
        public void Configure(EntityTypeBuilder<Contest> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.Name)
                   .HasMaxLength(256)
                   .IsRequired();
            builder.Property(x => x.Description)
                   .IsRequired();
            builder.Property(x => x.StartAt)
                   .IsRequired();
            builder.Property(x => x.EndAt)
                   .IsRequired();
            builder.Property(x => x.CreatedAt)
                   .IsRequired();
            builder.Property(x => x.UpdatedAt)
                   .IsRequired(false)
                   .HasDefaultValue(null);
        }
    }
}
