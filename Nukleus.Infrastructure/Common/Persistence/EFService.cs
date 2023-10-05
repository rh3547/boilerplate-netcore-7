using Nukleus.Application.Common.Services;
using Nukleus.Application.Common.Validation;
using MapsterMapper;
using Nukleus.Application.Common.Persistence;
using Nukleus.Infrastructure.Common.Services;
using System.Linq.Expressions;
using Nukleus.Domain.SeedWork;

namespace Nukleus.Infrastructure.Common.Persistence
{
    public class EFService<T> : IService<T> where T : Entity
    {
        private readonly INukleusLogger _logger;
        private readonly IRepository<T> _repository;
        private readonly IMapper _mapper;

        protected Expression<Func<T, bool>>? GetAllConstraint = null;
        protected Expression<Func<T, bool>>? GetByIdConstraint = null;
        protected Expression<Func<T, bool>>? SearchConstraint = null;
        protected Expression<Func<T, bool>>? AddConstraint = null;
        protected Expression<Func<T, bool>>? UpdateConstraint = null;
        protected Expression<Func<T, bool>>? PatchConstraint = null;
        protected Expression<Func<T, bool>>? DeleteConstraint = null;

        public EFService(INukleusLogger logger, IRepository<T> repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<Result<List<TResultDto>>> GetAllAsync<TResultDto>(int? pageSize, int? page, string[] include)
        {
            IQueryable<T> query = _repository.Query();

            query = ApplyGetAllConstraint(query);

            if (pageSize != null && page != null && pageSize > 0 && page > 0)
            {
                query = query.AddPaging((int)pageSize, (int)page);
            }

            if (include != null && include.Length > 0)
            {
                query = query.IncludeMultiple(include);
            }

            Result<List<T>> operation = await _repository.GetListAsync(query);

            if (operation.IsFaulted)
            {
                return operation.Error;
            }

            return _mapper.Map<List<TResultDto>>(operation.Value);
        }

        public virtual async Task<Result<TResultDto>> GetByIdAsync<TResultDto>(Guid id, string[] include)
        {
            IQueryable<T> query = _repository.Query().Where(x => x.Id == id);

            if (include != null && include.Length > 0)
            {
                query = query.IncludeMultiple(include);
            }

            Result<T> operation = await _repository.GetSingleAsync(query);

            if (operation.IsFaulted)
            {
                return operation.Error;
            }

            if (!TestGetByIdConstraint(operation.Value))
            {
                return Error.Unauthorized($"You do not have access to get that resource.");
            }

            return _mapper.Map<TResultDto>(operation.Value);
        }

        public virtual async Task<Result<List<TResultDto>>> SearchAsync<TResultDto, TSearchDto>(TSearchDto searchDTO, int? pageSize, int? page, string[] include)
        {
            IQueryable<T> query = _repository.Query().Where(PredicateBuilder.MapPredicate<T, TSearchDto>(searchDTO));

            query = ApplySearchConstraint(query);

            if (pageSize != null && page != null && pageSize > 0 && page > 0)
            {
                query = query.AddPaging((int)pageSize, (int)page);
            }

            if (include != null && include.Length > 0)
            {
                query = query.IncludeMultiple(include);
            }

            Result<List<T>> operation = await _repository.GetListAsync(query);

            if (operation.IsFaulted)
            {
                return operation.Error;
            }

            return _mapper.Map<List<TResultDto>>(operation.Value);
        }

        public virtual async Task<Result<TResultDto>> AddAsync<TResultDto, TAddDto>(TAddDto addDTO)
        {
            T entityToAdd = _mapper.Map<T>(addDTO);

            if (!TestAddConstraint(entityToAdd))
            {
                return Error.Unauthorized($"You do not have access to add that resource.");
            }

            Result<T> operation = await _repository.AddAsync(entityToAdd);

            if (operation.IsFaulted)
            {
                return operation.Error;
            }

            Result<int> saveOperation = await _repository.SaveChangesAsync();

            if (saveOperation.IsFaulted)
            {
                return saveOperation.Error;
            }

            return _mapper.Map<TResultDto>(operation.Value);
        }

        public virtual async Task<Result<TResultDto>> UpdateAsync<TResultDto>(TResultDto updateDTO)
        {
            T entityToUpdate = _mapper.Map<T>(updateDTO);

            if (!TestUpdateConstraint(entityToUpdate))
            {
                return Error.Unauthorized($"You do not have access to modify that resource.");
            }

            Result<T> operation = _repository.Update(entityToUpdate);

            if (operation.IsFaulted)
            {
                return operation.Error;
            }

            Result<int> saveOperation = await _repository.SaveChangesAsync();

            if (saveOperation.IsFaulted)
            {
                return saveOperation.Error;
            }

            return _mapper.Map<TResultDto>(operation.Value);
        }

        public virtual async Task<Result<TResultDto>> PatchAsync<TResultDto, TPatchDto>(Guid id, TPatchDto patchDTO)
        {
            Result<T> getEntityOperation = await _repository.FindAsync(id);

            if (!TestPatchConstraint(getEntityOperation.Value))
            {
                return Error.Unauthorized($"You do not have access to modify that resource.");
            }

            if (getEntityOperation.IsFaulted)
            {
                return getEntityOperation.Error;
            }

            T entityToUpdate = getEntityOperation.Value;

            foreach (var property in typeof(T).GetProperties())
            {
                var updateValue = typeof(TPatchDto).GetProperty(property.Name)?.GetValue(patchDTO, null);
                if (updateValue != null && !updateValue.Equals(property.GetValue(entityToUpdate)))
                {
                    property.SetValue(entityToUpdate, updateValue);
                }
            }

            Result<T> operation = _repository.Update(entityToUpdate);

            if (operation.IsFaulted)
            {
                return operation.Error;
            }

            Result<int> saveOperation = await _repository.SaveChangesAsync();

            if (saveOperation.IsFaulted)
            {
                return saveOperation.Error;
            }

            return _mapper.Map<TResultDto>(operation.Value);
        }

        public virtual async Task<Result<bool>> DeleteAsync(Guid id)
        {
            Result<T> entityToDelete = await _repository.FindAsync(id);

            if (!TestDeleteConstraint(entityToDelete.Value))
            {
                return Error.Unauthorized($"You do not have access to delete that resource.");
            }

            if (entityToDelete.IsFaulted)
            {
                return entityToDelete.Error;
            }

            Result<bool> operation = _repository.Delete(entityToDelete.Value);

            if (operation.IsFaulted)
            {
                return operation.Error;
            }

            Result<int> saveOperation = await _repository.SaveChangesAsync();

            if (saveOperation.IsFaulted)
            {
                return saveOperation.Error;
            }

            return operation.Value;
        }

        protected virtual void SetConstraints(Expression<Func<T, bool>> predicate)
        {
            GetAllConstraint = predicate;
            GetByIdConstraint = predicate;
            SearchConstraint = predicate;
            AddConstraint = predicate;
            UpdateConstraint = predicate;
            PatchConstraint = predicate;
            DeleteConstraint = predicate;
        }

        protected virtual void SetReadConstraints(Expression<Func<T, bool>> predicate)
        {
            GetAllConstraint = predicate;
            GetByIdConstraint = predicate;
            SearchConstraint = predicate;
        }

        protected virtual void SetWriteConstraints(Expression<Func<T, bool>> predicate)
        {
            AddConstraint = predicate;
            UpdateConstraint = predicate;
            PatchConstraint = predicate;
            DeleteConstraint = predicate;
        }

        protected virtual IQueryable<T> ApplyGetAllConstraint(IQueryable<T> query)
        {
            if (GetAllConstraint == null) return query;
            return query.Where(GetAllConstraint);
        }

        protected virtual IQueryable<T> ApplySearchConstraint(IQueryable<T> query)
        {
            if (SearchConstraint == null) return query;
            return query.Where(SearchConstraint);
        }

        protected virtual bool TestGetByIdConstraint(T entityToTest)
        {
            if (GetByIdConstraint == null) return true;
            return GetByIdConstraint.Compile().Invoke(entityToTest);
        }

        protected virtual bool TestAddConstraint(T entityToTest)
        {
            if (AddConstraint == null) return true;
            return AddConstraint.Compile().Invoke(entityToTest);
        }

        protected virtual bool TestUpdateConstraint(T entityToTest)
        {
            if (UpdateConstraint == null) return true;
            return UpdateConstraint.Compile().Invoke(entityToTest);
        }

        protected virtual bool TestPatchConstraint(T entityToTest)
        {
            if (PatchConstraint == null) return true;
            return PatchConstraint.Compile().Invoke(entityToTest);
        }

        protected virtual bool TestDeleteConstraint(T entityToTest)
        {
            if (DeleteConstraint == null) return true;
            return DeleteConstraint.Compile().Invoke(entityToTest);
        }
    }
}