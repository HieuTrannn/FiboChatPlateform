using Authentication.Domain.Abstraction;
using Authentication.Infrastructure.Data;
using Authentication.Infrastructure.Implementation;

public class UnitOfWork : IUnitOfWork
{
    private readonly AccountDbContext _dbContext;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(AccountDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IGenericRepository<T> GetRepository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new GenericRepository<T>(_dbContext);
        }
        return (IGenericRepository<T>)_repositories[type];
    }

    public void BeginTransaction() => _dbContext.Database.BeginTransaction();

    public void CommitTransaction() => _dbContext.Database.CommitTransaction();

    public void RollBack() => _dbContext.Database.RollbackTransaction();

    public void Save() => _dbContext.SaveChanges();

    public async Task SaveChangeAsync() => await _dbContext.SaveChangesAsync();

    public bool HasActiveTransaction() => _dbContext.Database.CurrentTransaction != null;
}
