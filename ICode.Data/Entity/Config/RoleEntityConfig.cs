using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Data.Entity.Config
{
    public class RoleEntityConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(role => role.ID);
            builder.Property(role => role.Name)
                  .HasMaxLength(8)
                  .IsRequired();
            builder.Property(role => role.Priority)
                  .IsRequired();
            builder.Property(role => role.CreatedAt)
                  .IsRequired();
            builder.Property(role => role.UpdatedAt)
                  .IsRequired(false)
                  .HasDefaultValue(null);
        }
    }
}
