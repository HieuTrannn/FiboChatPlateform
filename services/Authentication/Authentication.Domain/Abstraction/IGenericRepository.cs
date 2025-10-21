using Contracts.Common;
using System.Linq.Expressions;

namespace Authentication.Domain.Abstraction;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> GetQueryable(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null);
    IQueryable<T> Entities { get; }
    Task<T?> FindByConditionAsync(Expression<Func<T, bool>> predicate);
    Task<IList<T>> GetAllAsync(Expression<Func<IQueryable<T>, IQueryable<T>>>? include);
    Task<IList<T>> GetAllAsync();
    Task<BasePaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize);
    Task<T?> GetByIdNoTracking(object id);
    Task<T?> GetByIdAsync(object id);
    Task<T> InsertAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(object id);
    Task DeleteAsync(Expression<Func<T, bool>> predicate);
    Task SoftDeleteAsync(Guid id);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
    Task SaveAsync();

    Task InsertRangeAsync(IEnumerable<T> entities);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);
    Task<IList<T>> GetAllAsync(string? includeProperties = null);
}