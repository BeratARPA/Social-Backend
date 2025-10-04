using MediatR;

namespace UserService.Data.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
        public bool IsDeleted { get; protected set; } = false;
        public DateTime? DeletedAt { get; protected set; }
        public DateTime? ScheduledForHardDeleteAt { get; protected set; }


        private List<INotification> domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            domainEvents ??= new List<INotification>();
            domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            if (domainEvents == null) return;
            domainEvents.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            domainEvents?.Clear();
        }

        // Soft Delete işlemi
        public virtual void SoftDelete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            ScheduledForHardDeleteAt = DateTime.UtcNow.AddDays(30); // 30 gün sonra hard delete
            UpdatedAt = DateTime.UtcNow;
        }

        // Restore işlemi (kullanıcı geri döndüğünde)
        public virtual void Restore()
        {
            IsDeleted = false;
            DeletedAt = null;
            ScheduledForHardDeleteAt = null;
            UpdatedAt = DateTime.UtcNow;
        }

        // Hard delete zamanının gelip gelmediğini kontrol et
        public bool IsReadyForHardDelete()
        {
            return ScheduledForHardDeleteAt.HasValue &&
                   DateTime.UtcNow >= ScheduledForHardDeleteAt.Value;
        }
    }
}