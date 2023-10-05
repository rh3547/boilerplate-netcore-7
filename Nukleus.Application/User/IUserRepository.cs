using Nukleus.Application.Common.Persistence;
using Nukleus.Application.Common.Validation;
using Nukleus.Domain.Entities;

namespace Nukleus.Application.UserModule
{
    // Gets all the common functions defined in IRepository, plus custom functions for Users.
    // This will be what the repo implements to get both.
    public interface IUserRepository : IRepository<User>
    {
    }
}