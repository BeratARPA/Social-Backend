using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
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
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);

            // ⭐ GLOBAL QUERY FILTER - Tüm BaseEntity'lerden türeyenler için otomatik soft delete filtresi
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var body = Expression.Equal(
                        Expression.Property(parameter, nameof(BaseEntity.IsDeleted)),
                        Expression.Constant(false)
                    );
                    var lambda = Expression.Lambda(body, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        public void IgnoreQueryFilters()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
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
    }
}
