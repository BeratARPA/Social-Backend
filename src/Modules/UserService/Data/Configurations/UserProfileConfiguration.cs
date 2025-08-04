using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Data.Entities;

namespace UserService.Data.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("UserProfiles");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Username).IsUnique();

            builder.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.FullName).HasMaxLength(100);
            builder.Property(x => x.Bio).HasMaxLength(300);
            builder.Property(x => x.AvatarUrl).HasMaxLength(250);

            builder.Property(x => x.IsPrivate).HasDefaultValue(false);
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        }
    }
}
