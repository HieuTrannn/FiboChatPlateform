using Course.Domain.Entities;

namespace Course.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Topic> Topics { get; }
        IGenericRepository<Semester> Semesters { get; }
        IGenericRepository<Class> Classes { get; }
        IGenericRepository<ClassEnrollment> ClassEnrollments { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
