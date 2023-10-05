using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Nukleus.Application.Authentication;
using Nukleus.Application.Common.Services;
using Nukleus.Application.Common.Validation;
using Nukleus.Application.UserModule;
using Nukleus.Domain.Entities;

namespace Nukleus.Infrastructure.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _usersRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IHashingService _hashingService;
        private readonly ISession _session;
        private readonly IDateTimeProvider _dateTomeProvider;
        private readonly IEmailService _emailService;
        private readonly INukleusLogger _logger;

        public AuthenticationService(IUserRepository usersRepository,
                                     IJwtTokenService jwtTokenService,
                                     IHashingService hashingService,
                                     ISession session,
                                     IDateTimeProvider dateTimeProvider,
                                     IEmailService emailService,
                                     INukleusLogger logger)
        {
            _usersRepository = usersRepository;
            _jwtTokenService = jwtTokenService;
            _hashingService = hashingService;
            _session = session;
            _dateTomeProvider = dateTimeProvider;
            _emailService = emailService;
            _logger = logger;
        }
        public async Task<Result<TokenDTO>> Authenticate(AuthenticateDTO authData)
        {

            // Validate DTO not caught by model binding.
            if(authData.Username.IsNullOrEmpty() || authData.Password.IsNullOrEmpty())
            {
                return Error.ValidationError("Provided username or password was null.");
            }


            // Since user exists, get the user for password verification.
            IQueryable<User> query = _usersRepository.QueryWithTracking().Where(user => user.Username == authData.Username);
            Result<User?> usersOperation = await _usersRepository.GetSingleOrDefaultAsync(query);
            if(usersOperation.IsFaulted)
            {
                return usersOperation.Error;
            }

            if(usersOperation.Value is null)
            {
                return Error.Unauthorized("Invalid Credentials.");
            }

            // If the hashed, provided password matches the password in the db, continue.
            if(_hashingService.Verify(authData.Password, usersOperation.Value.Password))
            {
                // Issue tokens.
                var accessToken = _jwtTokenService.GenerateAccessToken(usersOperation.Value.Id);
                var refreshToken = _jwtTokenService.GenerateRefreshToken(usersOperation.Value.Id);

                // Store the token on the user and update DB.
                usersOperation.Value.RefreshToken = refreshToken;
                _usersRepository.Update(usersOperation.Value);
                await _usersRepository.SaveChangesAsync();

                return new TokenDTO { AccessToken = accessToken, RefreshToken = refreshToken };
            }
            else
            {
                return Error.Unauthorized("Invalid Credentials.");
            }
        }

        // https://cheatsheetseries.owasp.org/cheatsheets/Forgot_Password_Cheat_Sheet.html
        // https://cheatsheetseries.owasp.org/cheatsheets/Forgot_Password_Cheat_Sheet.html#url-tokens
        // TODO decide if we want to allow getting new email tokens if one is already out there and valid.
        public async Task<Result> ForgotPassword(ForgotPasswordDTO forgotPasswordData)
        {
            //const string SANITIZED_RESPONSE = "If the provided email exists, a reset email will be sent. Please check your inbox and follow the instructions in the email.";
            
            // Validate input
            if(forgotPasswordData.Email.IsNullOrEmpty())
            {
                return Error.ValidationError("Provided email was null or empty.");
            }

            // Check if email exists. If not, return uniform response.
            IQueryable<User> userExistsQuery = _usersRepository.Query().Where(user => (user.Email == forgotPasswordData.Email));
            Result<User?> userExistsOperation = await _usersRepository.GetSingleOrDefaultAsync(userExistsQuery);
            if (userExistsOperation.IsFaulted)
            {
                return userExistsOperation.Error;
            }
            if (userExistsOperation.Value is null)
            {
                // No user exists with that email. Return "success" (to obfuscate)
                return new Result(true);
            }

            // We know we have a valid User
            User currentUser = userExistsOperation.Value;

            // Create a cryptographically secure OTP. Set expiry, store on the user in question.
            string URLToken = GenerateUniqueBase64URLToken();
            DateTime expiry = _dateTomeProvider.UtcNow.AddSeconds(60); // TODO move to appsettings config.

            // Set reset password info.
            currentUser.ResetPasswordToken = URLToken;
            currentUser.ResetPasswordTokenExpiryTime = expiry;

            // Remove any refresh tokens out there.
            currentUser.RefreshToken = null;

            // Persist changes to db.
            _usersRepository.Update(currentUser);
            await _usersRepository.SaveChangesAsync();

            // Send the email
            // string frontendURLLink = $"https://localhost:4200/Authentication/reset-password?email={userExistsOperation.Value.Email}&token={URLToken}";
            // Email resetPasswordEmail = new Email
            // {
            //     To = userExistsOperation.Value.Email,
            //     Subject = "Nukleus - Reset Password",
            //     Content = 
            //     $@"Click here to reset your password for Nukleus: 
            //     {frontendURLLink}
            //     Do not share this link with anyone."
                
            // };
            // await _emailService.SendEmailAsync(resetPasswordEmail);

            _logger.LogDebug($"Generated secure unique reset token - {URLToken}");

            Result forgotPasswordEmailSendOperation = await _emailService.SendForgotPasswordEmailAsync("rdblack3@gmail.com",currentUser.Username,URLToken);
            if(!forgotPasswordEmailSendOperation.IsSuccessful)
            {
                return forgotPasswordEmailSendOperation;
            }
            return new Result(true);
        }

        public async Task<Result> Logout()
        {
            // Get the user from our verified access token. (should never be null)
            User? currentUser = _session.GetUser();
            if (currentUser is null)
            {
                _logger.LogDebug("Session variable for user was null.");
                return Error.Unauthorized("Invalid token credentials.");
            }

            currentUser.RefreshToken = null;
            _usersRepository.Update(currentUser);
            return await _usersRepository.SaveChangesAsync();
        }

        public async Task<Result<TokenDTO>> Refresh(TokenDTO tokenData)
        {
            // Validate DTO not caught by model binding.
            if (tokenData.RefreshToken.IsNullOrEmpty() || tokenData.AccessToken.IsNullOrEmpty())
            {
                return Error.ValidationError("Provided access or refresh tokens were null.");
            }

            // Get the user from our verified access token. (should never be null)
            User? currentUser = _session.GetUser();
            if (currentUser is null)
            {
                _logger.LogDebug("Session variable for user was null.");
                return Error.Unauthorized("Invalid token credentials.");
            }

            // Verify the info/signatures on the tokens are valid (valid JWT token)
            if(!await _jwtTokenService.VerifyToken(tokenData.RefreshToken) || !await _jwtTokenService.VerifyToken(tokenData.AccessToken, false))
            {
                // Token has been modified in some way.
                // Revoke access for the session user with the refresh token stored. (valid id/access token)
                currentUser.RefreshToken = null;
                _usersRepository.Update(currentUser);
                await _usersRepository.SaveChangesAsync();

                _logger.LogDebug("Refresh or access token verification failed.");
                return Error.Unauthorized("Invalid token credentials.");
            }

            // Get the user from the db with the provided refresh token. (could alternatively get by id provided)
            IQueryable<User> userExistsQuery = _usersRepository.QueryWithTracking().Where(user => (user.RefreshToken == tokenData.RefreshToken));
            Result<User?> userExistsOperation = await _usersRepository.GetSingleOrDefaultAsync(userExistsQuery);
            if(userExistsOperation.IsFaulted)
            {
                return userExistsOperation.Error;
            }
            // If we pass an invalid refresh token
            if(userExistsOperation.Value is null)
            {
                // Revoke access for the session user with the refresh token stored. (valid id/access token)
                currentUser.RefreshToken = null;
                _usersRepository.Update(currentUser);
                await _usersRepository.SaveChangesAsync();

                _logger.LogDebug("No user exists with the provided refresh token.");
                return Error.Unauthorized("Invalid token credentials.");
            }

            // If id we got from the verified access token (from controller attribute) is the same user with the provided refresh token.
            if(userExistsOperation.Value.Id == currentUser.Id)
            {
                // Issue a new refresh token and access token.
                var accessToken = _jwtTokenService.GenerateAccessToken(userExistsOperation.Value.Id);
                var refreshToken = _jwtTokenService.GenerateRefreshToken(userExistsOperation.Value.Id);

                // Store the token on the user and update DB.
                userExistsOperation.Value.RefreshToken = refreshToken;
                _usersRepository.Update(userExistsOperation.Value);
                await _usersRepository.SaveChangesAsync();

                return new TokenDTO { AccessToken = accessToken, RefreshToken = refreshToken };
            }
            else
            {
                // Revoke access for the session user AND/OR the user with the refresh token stored.
                await _usersRepository.BeginTransactionAsync();

                currentUser.RefreshToken = "";
                _usersRepository.Update(currentUser);
                await _usersRepository.SaveChangesAsync();

                userExistsOperation.Value.RefreshToken = null;
                _usersRepository.Update(userExistsOperation.Value);
                await _usersRepository.SaveChangesAsync();

                await _usersRepository.CommitTransactionAsync();

                _logger.LogDebug("User with the provided refresh token does not match the User with the userid provided on the session.");
                return Error.Unauthorized("Invalid token credentials.");
            }

        }

        public async Task<Result> ResetPassword(ResetPasswordDTO resetPasswordData)
        {
            // Validate DTO not caught by model binding.
            if (resetPasswordData.EmailToken.IsNullOrEmpty() || resetPasswordData.Password.IsNullOrEmpty())
            {
                return Error.ValidationError("Provided email token or password was null or empty.");
            }

            // Get the user from the db with matching unique email token
            IQueryable<User> userExistsQuery = _usersRepository.QueryWithTracking().Where(user => (user.ResetPasswordToken == resetPasswordData.EmailToken));
            Result<User?> userExistsOperation = await _usersRepository.GetSingleOrDefaultAsync(userExistsQuery);
            if (userExistsOperation.IsFaulted)
            {
                return userExistsOperation.Error;
            }
            // If this email token isn't on a user. (expired, already used, etc.)
            if (userExistsOperation.Value is null)
            {
                _logger.LogDebug("No user exists that has the provided email token.");
                return Error.Unauthorized();
            }

            // We have a valid user at this point.
            User currentUser = userExistsOperation.Value;

            // We know we have a user with a valid email token. Verify the email token has not yet expired.
            if(currentUser.ResetPasswordTokenExpiryTime >= _dateTomeProvider.UtcNow)
            {
                // Update Password
                currentUser.Password = _hashingService.Hash(resetPasswordData.Password);

                // Revoke any refresh tokens, remove the email reset token.
                currentUser.RefreshToken = null;
                currentUser.ResetPasswordToken = null;
                currentUser.ResetPasswordTokenExpiryTime = null;

                _usersRepository.Update(currentUser);
                _logger.LogDebug($"Updating new password for user {currentUser.AccountId} - ************");
                return await _usersRepository.SaveChangesAsync();
            }
            else
            {
                // Revoke any refresh tokens, remove the email reset token. (They'll have to request another password reset)
                currentUser.RefreshToken = null;
                currentUser.ResetPasswordToken = null;
                currentUser.ResetPasswordTokenExpiryTime = null;

                _usersRepository.Update(currentUser);
                await _usersRepository.SaveChangesAsync();

                _logger.LogDebug($"Reset password token is expired for user {currentUser.AccountId}. Revoked refresh and email tokens.");
                return Error.Unauthorized();
            }
        }

        private string GenerateUniqueBase64URLToken()
        {
            string refreshToken = "";
            using (var rng = RandomNumberGenerator.Create())
            {
                var randomNumber = new byte[64];
                rng.GetBytes(randomNumber);
                refreshToken = Convert.ToBase64String(randomNumber);
                return refreshToken;
            }
        }
    }
}