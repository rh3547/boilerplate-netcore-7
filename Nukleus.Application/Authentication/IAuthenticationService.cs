using Nukleus.Application.Common.Validation;

namespace Nukleus.Application.Authentication
{
    public interface IAuthenticationService
    {
        Task<Result<TokenDTO>> Authenticate(AuthenticateDTO authData);

        Task<Result<TokenDTO>> Refresh(TokenDTO tokenData);

        Task<Result> Logout();

        Task<Result> ForgotPassword(ForgotPasswordDTO forgotPasswordData);

        Task<Result> ResetPassword(ResetPasswordDTO resetPasswordData);
    }
}