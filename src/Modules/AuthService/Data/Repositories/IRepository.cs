using AuthService.Data.UnitOfWork;

namespace AuthService.Data.Repositories
{
    public interface IRepository<T>
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
