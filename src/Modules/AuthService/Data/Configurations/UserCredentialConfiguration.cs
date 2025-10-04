using AuthService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Data.Configurations
{
    public class UserCredentialConfiguration : IEntityTypeConfiguration<UserCredential>
    {
        public void Configure(EntityTypeBuilder<UserCredential> builder)
        {
            builder.ToTable("UserCredentials");

            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.Username)
                   .IsUnique();

            builder.Property(u => u.Username)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasMany(u => u.RefreshTokens)
                   .WithOne(r => r.UserCredential)
                   .HasForeignKey(r => r.UserCredentialId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
