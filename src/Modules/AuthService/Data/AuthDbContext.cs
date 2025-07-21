using AuthService.Data.Entities;
using AuthService.Data.UnitOfWork;
using AuthService.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public AuthDbContext() : base() { }

        public AuthDbContext(DbContextOptions<AuthDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new UserEntityConfigurations());
        }
    }
}
