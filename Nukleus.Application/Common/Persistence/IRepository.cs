using System.Linq.Expressions;
using Nukleus.Application.Common.Validation;

namespace Nukleus.Application.Common.Persistence
{
    // Common functions we want for everything.

    /* 
    Conventions:
    Method names and signatures should describe all of the functionality.
    For instance: 
        Result<T> implies null is not a valid option. It either succeeds and returns, or errors. 'GetById'
        Result<T?> implies null is a valid return type. The method name may be 'TryGetByID'
    */
    public interface IRepository<T>
    {
        public IQueryable<T> Query();
        public IQueryable<T> QueryWithTracking();
        public Task<Result<List<T>>> GetListAsync(IQueryable<T> query);
        public Task<Result<T?>> FindAsync(Guid id);
        public Task<Result<bool>> ExistsAsync(Expression<Func<T, bool>> predicate);
        public Task<Result<int>> CountAsync(IQueryable<T> query);
        public Task<Result<T>> AddAsync(T entity);
        public Result<T> Update(T entity);
        public Result<bool> Delete(T entity);
        public Task<Result<bool>> BeginTransactionAsync();
        public Task<Result<bool>> CommitTransactionAsync();
        public Task<Result<bool>> RollbackTransactionAsync();
        public Task<Result<bool>> DisposeTransactionAsync();
        public Task<Result<int>> SaveChangesAsync();

        public Result<List<T>> GetList(IQueryable<T> query);

        // Single
        public Result<T> GetSingle(IQueryable<T> query);
        public Task<Result<T>> GetSingleAsync(IQueryable<T> query);
        public Result<T?> GetSingleOrDefault(IQueryable<T> query);
        public Task<Result<T?>> GetSingleOrDefaultAsync(IQueryable<T> query);

        // First
        public Result<T> GetFirst(IQueryable<T> query);
        public Task<Result<T>> GetFirstAsync(IQueryable<T> query);
        public Result<T?> GetFirstOrDefault(IQueryable<T> query);
        public Task<Result<T?>> GetFirstOrDefaultAsync(IQueryable<T> query);

        public Result<T?> Find(Guid id);
        public Result<bool> Exists(Expression<Func<T, bool>> predicate);
        public Result<int> Count(IQueryable<T> query);
        public Result<T> Add(T entity);
        public Result<bool> BeginTransaction();
        public Result<bool> CommitTransaction();
        public Result<bool> RollbackTransaction();
        public Result<bool> DisposeTransaction();
        public Result<int> SaveChanges();
    }
}