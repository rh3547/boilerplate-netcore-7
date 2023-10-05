using Nukleus.Infrastructure.Common.Persistence;
using MapsterMapper;
using Nukleus.Application.AccountModule;
using Nukleus.Application.Common.Validation;
using Nukleus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Nukleus.Infrastructure.AccountModule
{
    internal class AccountRepository : EFRepository<Account>, IAccountRepository
    {
        public AccountRepository(NukleusDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}