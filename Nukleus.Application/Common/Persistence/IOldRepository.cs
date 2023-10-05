using System.Linq.Expressions;
using Nukleus.Application.Common.Validation;
using Nukleus.Domain.SeedWork;

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
    public interface IOldRepository<T>
    {
        Task<Result<List<T>>> GetAllAsync(int? pageSize, int? page, string[] include); // Only when you use when you actually want ALL. Should filter in db otherwise.
        Result<T> GetById(Guid id);
        Task<Result<T>> GetByIdAsync(Guid id);

        // Implies one can exist. Throws an exception if more than one exists.
        Task<Result<T>> SingleAsync(Expression<Func<T, bool>> predicate);

        // Implies multiple can exist
        Task<Result<T>> FirstAsync(Expression<Func<T, bool>> predicate);
        Task<Result<T>> GetByIdWithIncludesAsync(Guid id, string[] include);
        Task<Result<bool>> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<Result<List<T>>> SearchAsync(Expression<Func<T, bool>> predicate, int? pageSize = null, int? page = null, string[]? include = default);
        Task<Result<List<T>>> SearchIQueryableAsync(IQueryable<T> query, int? pageSize, int? page, string[] include);
        Task<Result<T>> AddAsync(T entity);
        Task<Result<T>> UpdateAsync(T entity);
        Task<Result<bool>> DeleteAsync(T entity);

        // Unit of Work Stuff
        Task<Result<bool>> BeginTransactionAsync();
        Task<Result<bool>> CommitTransactionAsync();
        Task<Result<bool>> RollbackTransactionAsync();
        Task<Result<int>> SaveChangesAsync();

    }
}