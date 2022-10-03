using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Data.Entity.Config
{
    public class UserEntityConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.ID);
            builder.HasOne(user => user.Role)
                  .WithMany(role => role.Users)
                  .HasForeignKey(user => user.RoleID);
            builder.Property(user => user.Username)
                  .HasMaxLength(16)
                  .IsRequired();
            builder.Property(user => user.Password)
                  .IsRequired();
            builder.Property(user => user.Email)
                  .IsRequired();
            builder.Property(user => user.Avatar)
                  .IsRequired();
            builder.Property(user => user.Gender)
                  .IsRequired();
            builder.Property(user => user.AllowNotification)
                  .IsRequired();
            builder.Property(user => user.CreatedAt)
                  .IsRequired();
            builder.Property(user => user.UpdatedAt)
                  .IsRequired(false)
                  .HasDefaultValue(null);
            builder.Property(user => user.RemindAt)
                  .IsRequired(false)
                  .HasDefaultValue(null);
            builder.Property(user => user.RoleID)
                  .IsRequired();
            builder.Property(user => user.ForgotPasswordToken)
                  .IsRequired(false);
            builder.Property(user => user.ForgotPasswordTokenCreatedAt)
                  .IsRequired(false);
            builder.Property(user => user.ForgotPasswordTokenExpireAt)
                  .IsRequired(false);
        }
    }
}
