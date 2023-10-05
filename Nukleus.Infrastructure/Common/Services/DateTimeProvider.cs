using Nukleus.Application.Common.Services;

namespace Nukleus.Infrastructure.Common.Services
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}