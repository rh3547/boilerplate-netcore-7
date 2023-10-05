using Nukleus.Application.Common.Validation;
using Nukleus.Application.UserModule;
using Nukleus.Domain.Entities;
using Nukleus.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using MapsterMapper;

namespace Nukleus.Infrastructure.UserModule
{
    internal class UserRepository : EFRepository<User>, IUserRepository
    {
        public UserRepository(NukleusDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        // Example showing an override of a base repository function
        //public override async Task<Result<User>> GetByIdAsync(Guid id)
        //{
        //    // Implementation here..
        //}
    }
}