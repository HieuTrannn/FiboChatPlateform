using Identity.Domain.Entities;

namespace Identity.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Semester> Semesters { get; }
        IGenericRepository<Class> Classes { get; }
        IGenericRepository<ClassEnrollment> ClassEnrollments { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
