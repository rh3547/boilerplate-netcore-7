
using System.Linq.Expressions;
using Nukleus.Application.Common.Persistence;
using Nukleus.Application.Common.Validation;
using Microsoft.EntityFrameworkCore;
using MapsterMapper;

namespace Nukleus.Infrastructure.Common.Persistence
{
    // https://deviq.com/design-patterns/repository-pattern
    // EFCore already provides the Repository and UnitOfWork via the DBContext and DBSets.
    // However, for testing and to decouple EFCore with our Infrastructure layer,
    // I decided to abstract this to an IRepository for our Application and Infrastructure layers.

    // For our purposes, "repository" is just a specific type of "service".
    // Services should be in the infrastructure layer

    // By Convention, SaveChanges() needs to be called seperately

    abstract internal class EFRepository<T> : IRepository<T> where T : class
    {
        protected readonly NukleusDbContext _dbContext;
        private readonly IMapper _mapper;

        public EFRepository(NukleusDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public virtual IQueryable<T> Query()
        {
            return _dbContext.Set<T>().AsNoTracking().AsQueryable();
        }

        public virtual IQueryable<T> QueryWithTracking()
        {
            return _dbContext.Set<T>().AsQueryable();
        }

        public virtual async Task<Result<List<T>>> GetListAsync(IQueryable<T> query)
        {
            try
            {
                return await query.ToListAsync();
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while getting {typeof(T)}'s from the database.");
            }
        }

        public virtual Result<List<T>> GetList(IQueryable<T> query)
        {
            try
            {
                return query.ToList();
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while getting {typeof(T)}'s from the database.");
            }
        }

        public virtual Result<T> GetFirst(IQueryable<T> query)
        {
            try
            {
                return query.First();
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while getting {typeof(T)}'s from the database.");
            }
        }

        public virtual async Task<Result<T>> GetFirstAsync(IQueryable<T> query)
        {
            try
            {
                return await query.FirstAsync();
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while getting {typeof(T)}'s from the database.");
            }
        }

        public virtual Result<T?> GetFirstOrDefault(IQueryable<T> query)
        {
            try
            {
                return query.FirstOrDefault();
            }
            catch (ArgumentNullException nullException)
            {
                return Error.GenericError($"Argument provided was null.");

            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while getting {typeof(T)}'s from the database.");
            }
        }

        public virtual async Task<Result<T?>> GetFirstOrDefaultAsync(IQueryable<T> query)
        {
            try
            {
                return await query.FirstOrDefaultAsync();
            }
            catch (ArgumentNullException)
            {
                return Error.GenericError($"Argument provided was null.");

            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while getting {typeof(T)}'s from the database.");
            }
        }

        public virtual async Task<Result<T>> GetSingleAsync(IQueryable<T> query)
        {
            try
            {
                return await query.SingleAsync();
            }
            catch (ArgumentNullException)
            {
                return Error.GenericError($"Argument provided was null.");

            }
            catch (InvalidOperationException)
            {
                return Error.GenericError($"Expected a single result, but multiple exist.");

            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while getting {typeof(T)}'s from the database.");
            }
        }

        public virtual Result<T> GetSingle(IQueryable<T> query)
        {
            try
            {
                return query.Single();
            }
            catch (ArgumentNullException)
            {
                return Error.GenericError($"Argument provided was null.");

            }
            catch (InvalidOperationException)
            {
                return Error.GenericError($"Expected a single result, but multiple exist.");

            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while getting {typeof(T)}'s from the database.");
            }
        }

        public virtual async Task<Result<T?>> GetSingleOrDefaultAsync(IQueryable<T> query)
        {
            try
            {
                return await query.SingleOrDefaultAsync();
            }
            catch (ArgumentNullException)
            {
                return Error.GenericError($"Argument provided was null.");

            }
            catch (InvalidOperationException)
            {
                return Error.GenericError($"Expected a single result, but multiple exist.");

            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while getting {typeof(T)}'s from the database.");
            }
        }

        public virtual Result<T?> GetSingleOrDefault(IQueryable<T> query)
        {
            try
            {
                return query.SingleOrDefault();
            }
            catch (ArgumentNullException)
            {
                return Error.GenericError($"Argument provided was null.");

            }
            catch (InvalidOperationException)
            {
                return Error.GenericError($"Expected a single result, but multiple exist.");

            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while getting {typeof(T)}'s from the database.");
            }
        }

        public virtual async Task<Result<T?>> FindAsync(Guid id)
        {
            try
            {
                return await _dbContext.Set<T>().FindAsync(id);
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while finding a {typeof(T)} in the database.");
            }
        }

        public virtual Result<T?> Find(Guid id)
        {
            try
            {
                return _dbContext.Set<T>().Find(id);
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while finding a {typeof(T)} in the database.");
            }
        }

        public virtual async Task<Result<bool>> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                bool anyExist = await _dbContext.Set<T>().AnyAsync(predicate);
                return anyExist;
            }
            catch (ArgumentNullException)
            {
                return Error.GenericError($"Argument provided was null.");

            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while checking if any {typeof(T)}'s satisfy the provided condition.");
            }
        }

        public virtual Result<bool> Exists(Expression<Func<T, bool>> predicate)
        {
            try
            {
                bool anyExist = _dbContext.Set<T>().Any(predicate);
                return anyExist;
            }
            catch (ArgumentNullException)
            {
                return Error.GenericError($"Argument provided was null.");

            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while checking if any {typeof(T)}'s satisfy the provided condition.");
            }
        }

        public virtual async Task<Result<int>> CountAsync(IQueryable<T> query)
        {
            try
            {
                return await query.CountAsync();
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while getting the count of {typeof(T)}'s from the database.");
            }
        }

        public virtual Result<int> Count(IQueryable<T> query)
        {
            try
            {
                return query.Count();
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while getting the count of {typeof(T)}'s from the database.");
            }
        }

        public virtual async Task<Result<T>> AddAsync(T entity)
        {
            try
            {
                await _dbContext.Set<T>().AddAsync(entity);
                return entity;
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while adding {typeof(T)} to the database.");
            }
        }

        public virtual Result<T> Add(T entity)
        {
            try
            {
                _dbContext.Set<T>().Add(entity);
                return entity;
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while adding {typeof(T)} to the database.");
            }
        }

        public virtual Result<T> Update(T entity)
        {
            try
            {
                _dbContext.Set<T>().Update(entity);
                return entity;
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while updating {typeof(T)} in the database.");
            }
        }

        public virtual Result<bool> Delete(T entity)
        {
            try
            {
                _dbContext.Set<T>().Attach(entity);
                _dbContext.Set<T>().Remove(entity);
                return true;
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while deleting {typeof(T)} from the database.");
            }
        }

        public virtual async Task<Result<bool>> BeginTransactionAsync()
        {
            try
            {
                await _dbContext.Database.BeginTransactionAsync();
                return true;
            }
            catch (OperationCanceledException e)
            {
                return Error.CaughtExceptionError($"An operation cancelled exception was thrown while beginning the transaction.");
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while beginning the transaction.");
            }
        }

        public virtual Result<bool> BeginTransaction()
        {
            try
            {
                _dbContext.Database.BeginTransaction();
                return true;
            }
            catch (OperationCanceledException e)
            {
                return Error.CaughtExceptionError($"An operation cancelled exception was thrown while beginning the transaction.");
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while beginning the transaction.");
            }
        }

        public virtual async Task<Result<bool>> CommitTransactionAsync()
        {
            try
            {
                if (_dbContext.Database.CurrentTransaction != null)
                {
                    await _dbContext.Database.CurrentTransaction.CommitAsync();
                    return true;
                }
                return false;
            }
            catch (OperationCanceledException e)
            {
                return Error.CaughtExceptionError($"An operation cancelled exception was thrown while commiting the transaction.");
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while commiting the transaction.");
            }
        }

        public virtual Result<bool> CommitTransaction()
        {
            try
            {
                if (_dbContext.Database.CurrentTransaction != null)
                {
                    _dbContext.Database.CurrentTransaction.Commit();
                    return true;
                }
                return false;
            }
            catch (OperationCanceledException e)
            {
                return Error.CaughtExceptionError($"An operation cancelled exception was thrown while commiting the transaction.");
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while commiting the transaction.");
            }
        }

        public virtual async Task<Result<bool>> RollbackTransactionAsync()
        {
            try
            {
                if (_dbContext.Database.CurrentTransaction != null)
                {
                    await _dbContext.Database.CurrentTransaction.RollbackAsync();
                    return true;
                }
                return false;
            }
            catch (OperationCanceledException e)
            {
                return Error.CaughtExceptionError($"An operation cancelled exception was thrown while commiting the transaction.");
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while commiting the transaction.");
            }
        }

        public virtual Result<bool> RollbackTransaction()
        {
            try
            {
                if (_dbContext.Database.CurrentTransaction != null)
                {
                    _dbContext.Database.CurrentTransaction.Rollback();
                    return true;
                }
                return false;
            }
            catch (OperationCanceledException e)
            {
                return Error.CaughtExceptionError($"An operation cancelled exception was thrown while commiting the transaction.");
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while commiting the transaction.");
            }
        }

        public virtual async Task<Result<bool>> DisposeTransactionAsync()
        {
            try
            {
                if (_dbContext.Database.CurrentTransaction != null)
                {
                    await _dbContext.Database.CurrentTransaction.DisposeAsync();
                    return true;
                }
                return false;
            }
            catch (OperationCanceledException e)
            {
                return Error.CaughtExceptionError($"An operation cancelled exception was thrown while commiting the transaction.");
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while disposing the transaction.");
            }
        }

        public virtual Result<bool> DisposeTransaction()
        {
            try
            {
                if (_dbContext.Database.CurrentTransaction != null)
                {
                    _dbContext.Database.CurrentTransaction.Dispose();
                    return true;
                }
                return false;
            }
            catch (OperationCanceledException e)
            {
                return Error.CaughtExceptionError($"An operation cancelled exception was thrown while commiting the transaction.");
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while disposing the transaction.");
            }
        }

        public virtual async Task<Result<int>> SaveChangesAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while commiting the transaction.");
            }
        }

        public virtual Result<int> SaveChanges()
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch
            {
                return Error.UnknownError($"An unknown error occured while commiting the transaction.");
            }
        }
    }
}