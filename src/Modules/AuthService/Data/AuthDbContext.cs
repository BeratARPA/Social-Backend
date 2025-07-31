using AuthService.Data.Entities;
using AuthService.Data.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public AuthDbContext() : base() { }

        public AuthDbContext(DbContextOptions<AuthDbContext> options, IMediator mediator)
             : base(options) => _mediator = mediator;

        public DbSet<EmailConfirmationCode> EmailConfirmationCodes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);        

            modelBuilder.Entity<UserCredential>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(r => r.UserCredential)
                .HasForeignKey(r => r.UserCredentialId);

            modelBuilder.Entity<UserCredential>()
                .HasIndex(u => u.Username)
                .IsUnique();
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
