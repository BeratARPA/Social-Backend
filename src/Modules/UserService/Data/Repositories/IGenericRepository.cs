using UserService.Data.Entities;
using System.Linq.Expressions;

namespace UserService.Data.Repositories
{
    public interface IGenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includes = null,
            int? skip = null,
            int? take = null);

        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includes = null);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(Guid id);

        Task<T?> GetByIdIncludeDeletedAsync(Guid id);

        Task<IEnumerable<T>> GetIncludeDeletedAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

        Task<IEnumerable<T>> GetDeletedOnlyAsync(Expression<Func<T, bool>>? filter = null);

        Task SoftDeleteAsync(Guid id);

        Task RestoreAsync(Guid id);
    }
}
