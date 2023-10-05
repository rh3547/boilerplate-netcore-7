using Nukleus.Domain.Entities;
using Mapster;

namespace Nukleus.Application.AccountModule
{
    public class AccountMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Example mappings. Not needed for models that have exactly matching field names.
            //config.ForType<AccountSingleDTO, Account>().MapToConstructor(true);
            //config.ForType<Account, AccountSingleDTO>().MapToConstructor(true);
        }
    }
}