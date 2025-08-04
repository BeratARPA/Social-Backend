using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Data.Entities;

namespace UserService.Data.Configurations
{
    public class FollowConfiguration : IEntityTypeConfiguration<Follow>
    {
        public void Configure(EntityTypeBuilder<Follow> builder)
        {
            builder.ToTable("Follows");

            builder.HasKey(x => x.Id);

            builder.HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Followee)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(f => new { f.FollowerId, f.FolloweeId }).IsUnique();

            builder.Property(f => f.IsAccepted).HasDefaultValue(false);
            builder.Property(f => f.RequestedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
