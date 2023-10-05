using Nukleus.API.Common.BaseController;
using Nukleus.Application.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Nukleus.Domain.Entities;
using Nukleus.Application.AccountModule;
using Nukleus.Application.Common.Validation;
using Nukleus.API.Common.BaseController.Attributes;
using Nukleus.API.Common.Helpers;

namespace Nukleus.API.UserModule
{
    [Route("[controller]")]
    public class AccountController : GenericController<Account, AccountSingleDTO, AccountSearchResultDTO, AccountSearchDTO, AccountAddDTO, AccountPatchDTO>
    {
        private readonly IAccountService _service;
        private readonly INukleusLogger _logger;

        public AccountController(IAccountService service, INukleusLogger logger) : base(service, logger)
        {
            _service = service;
            _logger = logger;
        }

        [AllowAnonymous]
        public override async Task<IActionResult> GetById(Guid id, [ModelBinder(BinderType = typeof(StringArrayModelBinder))] string[] include)
        {
            return await base.GetById(id, include);
        }

        [HttpPost]
        [Route("new")]
        [AllowAnonymous]
        public async Task<IActionResult> NewAccount(NewAccountDTO dto)
        {
            Result<NewAccountResponseDTO> operation = await _service.NewAccount(dto);
            return FromResult(operation);
        }
    }
}