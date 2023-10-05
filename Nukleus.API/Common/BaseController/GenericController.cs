
using Nukleus.Application.Common.Services;
using Nukleus.Application.Common.Validation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Nukleus.Application.Common.Persistence;
using Nukleus.API.Common.Helpers;
using Nukleus.API.Common.BaseController.Attributes;

namespace Nukleus.API.Common.BaseController
{
    [Route("[controller]")]
    public class GenericController<T, TSingleResultDto, TSearchResultDto, TSearchDto, TAddDto, TPatchDto> : NukleusController
        where T : class
        where TSingleResultDto : class
        where TSearchResultDto : class
        where TSearchDto : class
        where TAddDto : class
        where TPatchDto : class
    {
        private readonly IService<T> _service;
        private readonly INukleusLogger _logger;

        public GenericController(IService<T> service, INukleusLogger logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [AuthenticateToken]
        public virtual async Task<IActionResult> GetAll([FromQuery] int pageSize, [FromQuery] int page, [ModelBinder(BinderType = typeof(StringArrayModelBinder))] string[] include)
        {
            Result<List<TSingleResultDto>> result = await _service.GetAllAsync<TSingleResultDto>(pageSize, page, include);
            return FromResult(result);
        }

        [HttpGet("{id}")]
        [AuthenticateToken]
        public virtual async Task<IActionResult> GetById(Guid id, [ModelBinder(BinderType = typeof(StringArrayModelBinder))] string[] include)
        {
            Result<TSingleResultDto> result = await _service.GetByIdAsync<TSingleResultDto>(id, include);
            return FromResult(result);
        }

        [HttpPost("Search")]
        [AuthenticateToken]
        public virtual async Task<IActionResult> Search(TSearchDto searchDTO, [FromQuery] int pageSize, [FromQuery] int page, [ModelBinder(BinderType = typeof(StringArrayModelBinder))] string[] include)
        {
            Result<List<TSearchResultDto>> result = await _service.SearchAsync<TSearchResultDto, TSearchDto>(searchDTO, pageSize, page, include);
            return FromResult(result);
        }

        [HttpPost]
        [AuthenticateToken]
        public virtual async Task<IActionResult> Add(TAddDto addDTO)
        {
            Result<TSingleResultDto> result = await _service.AddAsync<TSingleResultDto, TAddDto>(addDTO);
            return FromResult(result, HttpStatusCode.Created);
        }

        [HttpPut]
        [AuthenticateToken]
        public virtual async Task<IActionResult> Update(TSingleResultDto updateDTO)
        {
            Result<TSingleResultDto> result = await _service.UpdateAsync<TSingleResultDto>(updateDTO);
            return FromResult(result);
        }

        [HttpPatch("{id}")]
        [AuthenticateToken]
        public virtual async Task<IActionResult> Patch(Guid id, TPatchDto patchDTO)
        {
            Result<TSingleResultDto> result = await _service.PatchAsync<TSingleResultDto, TPatchDto>(id, patchDTO);
            return FromResult(result);
        }

        [HttpDelete("{id}")]
        [AuthenticateToken]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            Result<bool> result = await _service.DeleteAsync(id);
            return FromResult(result);
        }
    }
}