using Course.Domain.Entities;
using Course.Infrastructure.Interfaces;
using Course.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Course.Infrastructure.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CourseDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        public UnitOfWork(CourseDbContext context)
        {
            _context = context;
        }

        private IGenericRepository<Topic>? _topics;
        public IGenericRepository<Topic> Topics => _topics ??= new GenericRepository<Topic>(_context);

        private IGenericRepository<Semester>? _semesters;
        public IGenericRepository<Semester> Semesters => _semesters ??= new GenericRepository<Semester>(_context);

        private IGenericRepository<Class>? _classes;
        public IGenericRepository<Class> Classes => _classes ??= new GenericRepository<Class>(_context);

        private IGenericRepository<ClassEnrollment>? _classEnrollments;
        public IGenericRepository<ClassEnrollment> ClassEnrollments => _classEnrollments ??= new GenericRepository<ClassEnrollment>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}
