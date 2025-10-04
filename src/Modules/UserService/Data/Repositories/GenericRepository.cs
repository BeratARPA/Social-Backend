using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UserService.Data.Entities;
using UserService.Data.UnitOfWork;

namespace UserService.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly UserDbContext _context;

        public GenericRepository(UserDbContext context)
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
            var dbEntity = await _context.Set<T>().FindAsync(entity.Id);
            if (dbEntity == null)
                throw new KeyNotFoundException($"Entity with Id {entity.Id} not found.");

            _context.Entry(dbEntity).CurrentValues.SetValues(entity);
        }

        public async Task<T?> GetByIdIncludeDeletedAsync(Guid id)
        {
            return await _context.Set<T>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<T>> GetIncludeDeletedAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = _context.Set<T>().IgnoreQueryFilters();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetDeletedOnlyAsync(
            Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _context.Set<T>()
                .IgnoreQueryFilters()
                .Where(e => e.IsDeleted);

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                entity.SoftDelete();
                await UpdateAsync(entity);
            }
        }

        public async Task RestoreAsync(Guid id)
        {
            var entity = await GetByIdIncludeDeletedAsync(id);
            if (entity != null && entity.IsDeleted)
            {
                entity.Restore();
                await UpdateAsync(entity);
            }
        }
    }
}
