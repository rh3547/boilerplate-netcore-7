using Amazon;
using Amazon.Runtime.Internal.Util;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Nukleus.Application.Common.Services;
using Nukleus.Application.Common.Validation;

namespace Nukleus.Infrastructure.Common.Services
{
    public class AmazonSESEmailService : IEmailService
    {
        private readonly string _awsAccessKeyId = "";
        private readonly string _awsSecretAccessKey = "";
        private readonly RegionEndpoint _region = RegionEndpoint.USEast2;

        private readonly INukleusLogger _logger;

        public AmazonSESEmailService(INukleusLogger logger)
        {
            _logger = logger;
        }

        // aws ses create-template --cli-input-json file://C:\Projects\dotnet-seven-webapi-boilerplate\BackendBoilerplate\CleanArchitectureBoilerplate\Nukleus.Infrastructure\Common\Services\AWSEmailTemplates\ForgotPasswordEmailTemplate.json
        // aws ses update-template --cli-input-json file://{filepath}
        public async Task<Result> SendEmailAsync(string senderAddress, string receiverAddress, string subject, string body)
        {
            using (var client = new AmazonSimpleEmailServiceClient(_awsAccessKeyId, _awsSecretAccessKey, _region))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = senderAddress,
                    Destination = new Destination { ToAddresses = new List<string> { receiverAddress } },
                    Message = new Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = body
                            },
                            Text = new Content
                            {
                                Charset = "UTF-8",
                                Data = body
                            }
                        }
                    }
                };
                try
                {
                    Console.WriteLine("Sending email using Amazon SES...");
                    var response = await client.SendEmailAsync(sendRequest);
                    
                    Console.WriteLine("The email was sent successfully.");
                    return new Result(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);
                    return new Result(false,Error.UnknownError("Failed to send email. "));
                }
            }
        }
        public async Task<Result> SendForgotPasswordEmailAsync(string receiverAddress, string userName, string resetToken)
        {
            //string resetFrontendBaseURL = $"https://brewtime.io/forgot-password";
            string resetFrontendBaseURL = $"https://localhost:4200/Authentication/reset-password?email={receiverAddress}&token={resetToken}";
            using (var client = new AmazonSimpleEmailServiceClient(_awsAccessKeyId, _awsSecretAccessKey, _region))
            {
                var sendRequest = new SendTemplatedEmailRequest
                {
                    //Source = "no-reply@brewtime.io", // It must be a verified email in SES
                    Source = "rdblack3@gmail.com",
                    Destination = new Destination { ToAddresses = new List<string> { receiverAddress } },
                    Template = "ForgotPassword",
                    TemplateData = $"{{\"user_name\":\"{userName}\", \"reset_link\":\"{resetFrontendBaseURL + "?resetToken=" + resetToken}\"}}",
                };
                try
                {
                    _logger.LogDebug($"Sending forgot password email to {userName} at {receiverAddress}...");
                    var response = await client.SendTemplatedEmailAsync(sendRequest);
                    _logger.LogDebug($"Sent forgot password email to {userName} at {receiverAddress}.");
                    return new Result(true);
                }
                catch (Exception ex)
                {
                    _logger.LogUnknownError($"Error sending forgot password email to {userName} at {receiverAddress}: " + ex.Message);
                    return new Result(false, Error.UnknownError("Failed to send email. "));
                }
            }
        }
    }
}