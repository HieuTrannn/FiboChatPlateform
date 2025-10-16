using Authentication.Domain.Abstraction;
using Authentication.Infrastructure.Data;
using Contracts.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Authentication.Infrastructure.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AccountDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AccountDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> Entities => _context.Set<T>();
        public async Task<T?> FindByConditionAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<IQueryable<T>, IQueryable<T>>>? include)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include.Compile()(query);
            }

            return await query.ToListAsync();
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<BasePaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize)
        {
            query = query.AsNoTracking();
            int count = await query.CountAsync();
            IReadOnlyCollection<T> items = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
            return new BasePaginatedList<T>(items, count, index, pageSize);
        }

        public async Task<T?> GetByIdNoTracking(object id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }

            return entity;
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> InsertAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public Task UpdateAsync(T entity)
        {
            return Task.FromResult(_dbSet.Update(entity));
        }

        public async Task DeleteAsync(object id)
        {
            T entity = await _dbSet.FindAsync(id) ?? throw new KeyNotFoundException();
            _dbSet.Remove(entity);

        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                var statusProperty = typeof(T).GetProperty("Status");
                if (statusProperty != null && statusProperty.CanWrite)
                {
                    if (statusProperty.PropertyType.IsEnum)
                    {
                        // Map to Disabled enum value (case-insensitive)
                        var enumValue = Enum.Parse(statusProperty.PropertyType, "Disabled", ignoreCase: true);
                        statusProperty.SetValue(entity, enumValue);
                    }
                    else if (statusProperty.PropertyType == typeof(string))
                    {
                        statusProperty.SetValue(entity, "disabled");
                    }
                    else
                    {
                        _dbSet.Remove(entity); // fallback hard delete
                    }

                    await _context.SaveChangesAsync();
                }
                else
                {
                    _dbSet.Remove(entity); // fallback hard delete
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task InsertRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IList<T>> GetAllAsync(string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.ToListAsync();
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            var entities = await _dbSet.Where(predicate).ToListAsync();
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
            }
        }

    }
}
