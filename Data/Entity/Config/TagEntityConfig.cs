using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class TagEntityConfig : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(tag => tag.ID);
            builder.Property(tag => tag.Name)
                  .HasMaxLength(16)
                  .IsRequired();
            builder.Property(tag => tag.CreatedAt)
                  .IsRequired();
            builder.Property(tag => tag.UpdatedAt)
                  .IsRequired(false)
                  .HasDefaultValue(null);
        }
    }
}
