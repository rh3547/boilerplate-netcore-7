using Nukleus.Application.Common.Persistence;
using Nukleus.Application.Common.Validation;
using Nukleus.Domain.Entities;

namespace Nukleus.Application.UserModule
{
    public interface IUserService : IService<User>
    {
        public Task<Result<bool>> DoesEmailExist(string email);
        public Task<Result<bool>> DoesUsernameExist(string username);
    }
}