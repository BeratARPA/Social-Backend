using UserService.Data.UnitOfWork;

namespace UserService.Data.Repositories
{
    public interface IRepository<T>
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
