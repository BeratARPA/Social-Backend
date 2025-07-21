using AuthService.Data.UnitOfWork;
using AuthService.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuthService.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AuthDbContext _context;

        public GenericRepository(AuthDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
        {
            return await _context.Set<T>().CountAsync(filter);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _context.Set<T>().Where(e => e.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null, string? includes = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(includes))
                query = query.Include(includes);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includes = null, int? skip = null, int? take = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(includes))
            {
                foreach (var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include.Trim());
                }
            }

            if (orderBy != null)
                query = orderBy(query);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            await _context.Set<T>().Where(e => e.Id == entity.Id).ExecuteUpdateAsync(
                  u => u.SetProperty(e => e, entity));
        }
    }
}
