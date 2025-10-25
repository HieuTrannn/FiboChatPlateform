namespace Authentication.Domain.Abstraction;

public interface IUnitOfWork
{
    IGenericRepository<T> GetRepository<T>() where T : class;
    void Save();
    Task SaveChangeAsync();
    void BeginTransaction();
    void CommitTransaction();
    void RollBack();

    bool HasActiveTransaction();
}