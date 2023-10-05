using Nukleus.Application.Common.Validation;

namespace Nukleus.Application.Common.Services
{
    public class Email
    {
        public string To { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
    
    public interface IEmailService
    {
        public Task<Result> SendEmailAsync(string senderAddress, string receiverAddress, string subject, string body);

        public Task<Result> SendForgotPasswordEmailAsync(string receiverAddress, string userName, string resetToken);
    }
}