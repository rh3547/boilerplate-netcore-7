using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nukleus.Application.Common.Services
{
    // Will be implemented in Infrastructure layer.
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}