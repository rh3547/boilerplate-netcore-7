
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nukleus.API.Common.BaseController;
using Nukleus.API.Common.BaseController.Attributes;
using Nukleus.Application.Authentication;
using Nukleus.Application.Common.Validation;

namespace Nukleus.API.Authentication
{
    [Route("[controller]")]
    public class AuthenticationController : NukleusController
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authService = authenticationService;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateDTO authData)
        {
            Result<TokenDTO> operation = await _authService.Authenticate(authData);
            return FromResult(operation);
        }

        [HttpPost("refresh")]
        [AuthenticateTokenNoExp]
        public async Task<IActionResult> Refresh([FromBody] TokenDTO tokenData)
        {
            Result<TokenDTO> operation = await _authService.Refresh(tokenData);
            return FromResult(operation);
        }

        [HttpPost("logout")]
        [AuthenticateToken]
        public async Task<IActionResult> Logout()
        {
            Result operation = await _authService.Logout();
            return FromResult(operation);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordData)
        {
            Result operation = await _authService.ForgotPassword(forgotPasswordData);
            return FromResult(operation,HttpStatusCode.Accepted);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordData)
        {
            Result operation = await _authService.ResetPassword(resetPasswordData);
            return FromResult(operation);
        }
        
    }
}