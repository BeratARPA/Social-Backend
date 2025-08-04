using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Data.Entities;
using UserService.Data.UnitOfWork;

namespace UserService.Data
{
    public class UserDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public UserDbContext() : base() { }

        public UserDbContext(DbContextOptions<UserDbContext> options, IMediator mediator)
             : base(options) => _mediator = mediator;

        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await base.SaveChangesAsync(cancellationToken);

            var domainEntities = ChangeTracker
             .Entries<BaseEntity>()
             .Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any())
             .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await _mediator.Publish(domainEvent);

            return true;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await base.SaveChangesAsync(cancellationToken);
    }
}
