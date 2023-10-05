using Nukleus.Application.Common.Persistence;
using Nukleus.Application.Common.Validation;
using Nukleus.Domain.Entities;

namespace Nukleus.Application.AccountModule
{
    public interface IAccountService : IService<Account>
    {
        Task<Result<NewAccountResponseDTO>> NewAccount(NewAccountDTO dto);
    }
}