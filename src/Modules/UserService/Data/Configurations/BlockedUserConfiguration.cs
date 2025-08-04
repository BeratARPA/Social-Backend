using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Data.Entities;

namespace UserService.Data.Configurations
{
    public class BlockedUserConfiguration : IEntityTypeConfiguration<BlockedUser>
    {
        public void Configure(EntityTypeBuilder<BlockedUser> builder)
        {
            builder.ToTable("BlockedUsers");

            builder.HasKey(x => x.Id);

            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict); // ❗

            builder.HasOne(b => b.Blocked)
                .WithMany()
                .HasForeignKey(b => b.BlockedUserId)
                .OnDelete(DeleteBehavior.Restrict); // ❗

            builder.HasIndex(b => new { b.UserId, b.BlockedUserId }).IsUnique();
        }
    }
}
