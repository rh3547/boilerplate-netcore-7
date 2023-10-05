using Nukleus.Domain.Entities;
using Mapster;

namespace Nukleus.Application.UserModule
{
    public class UserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Example mappings. Not needed for models that have exactly matching field names.
            //config.ForType<UserSingleDTO, User>().MapToConstructor(true);
            //config.ForType<User, UserSingleDTO>().MapToConstructor(true);
        }
    }
}