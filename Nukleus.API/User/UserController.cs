
using Nukleus.API.Common.BaseController;
using Nukleus.Application.Common.Services;
using Nukleus.Application.Common.Validation;
using Nukleus.Application.UserModule;
using Microsoft.AspNetCore.Mvc;
using Nukleus.Domain.Entities;
using Nukleus.API.Common.Helpers;
using Nukleus.API.Common.BaseController.Attributes;

namespace Nukleus.API.UserModule
{
    // Controller base gives us "OK() and BadRequest, 
    // Wrappers around OkObjectResult and BadRequestResult, etc
    [Route("[controller]")]
    public class UserController : GenericController<User, UserSingleDTO, UserSearchResultDTO, UserSearchDTO, UserAddDTO, UserPatchDTO> 
    {
        private readonly IUserService _service;
        private readonly INukleusLogger _logger;

        public UserController(IUserService service, INukleusLogger logger) : base(service, logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("CheckEmail/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> DoesEmailExist(string email)
        {
            Result<bool> result = await _service.DoesEmailExist(email);
            return FromResult(result);
        }

        [HttpGet("CheckUsername/{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> DoesUsernameExist(string username)
        {
            Result<bool> result = await _service.DoesUsernameExist(username);
            return FromResult(result);
        }
    }
}