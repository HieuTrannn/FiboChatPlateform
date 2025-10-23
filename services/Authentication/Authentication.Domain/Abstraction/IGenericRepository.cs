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
    Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
    Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);
    Task<BasePaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize);
    Task<T?> GetByIdNoTracking(object id);
    Task<T?> GetByIdAsync(object id);
    Task<T?> GetByIdWithIncludeAsync(Guid id, params Expression<Func<T, object>>[] includes);
    Task<T> InsertAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(object id);
    Task DeleteAsync(Expression<Func<T, bool>> predicate);
    Task DeleteManyAsync(Expression<Func<T, bool>> predicate);
    Task DeleteRangeAsync(Expression<Func<T, bool>> predicate);
    Task SoftDeleteAsync(Guid id);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
    Task SaveAsync();
    Task InsertRangeAsync(IEnumerable<T> entities);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);
    Task<IList<T>> GetAllAsync(string? includeProperties = null);
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<IList<T>> FilterByAsync(Expression<Func<T, bool>> predicate);
}