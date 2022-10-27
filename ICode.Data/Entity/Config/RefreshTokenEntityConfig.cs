using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Config
{
    public class RefreshTokenEntityConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(token => token.ID);
            builder.Property(token => token.Token)
                  .IsRequired();
            builder.Property(token => token.State)
                  .HasDefaultValue(false)
                  .IsRequired();
            builder.Property(token => token.JwtID)
                  .IsRequired();
            builder.Property(token => token.UserID)
                  .IsRequired();
            builder.Property(token => token.JwtID)
                  .IsRequired();
            builder.HasOne(token => token.User)
                  .WithMany(user => user.Tokens)
                  .HasForeignKey(token => token.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
