using Nukleus.Application.Common.Validation;

namespace Nukleus.Application.Common.Persistence
{
    public interface IService<T>
    {
        public Task<Result<List<TResultDto>>> GetAllAsync<TResultDto>(int? pageSize, int? page, string[] include);
        public Task<Result<TResultDto>> GetByIdAsync<TResultDto>(Guid id, string[] include);
        public Task<Result<List<TResultDto>>> SearchAsync<TResultDto, TSearchDto>(TSearchDto searchDTO, int? pageSize, int? page, string[] include);
        public Task<Result<TResultDto>> AddAsync<TResultDto, TAddDto>(TAddDto addDTO);
        public Task<Result<TResultDto>> UpdateAsync<TResultDto>(TResultDto updateDTO);
        public Task<Result<TResultDto>> PatchAsync<TResultDto, TPatchDto>(Guid id, TPatchDto patchDTO);
        public Task<Result<bool>> DeleteAsync(Guid id);
    }
}