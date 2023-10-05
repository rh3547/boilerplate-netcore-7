using Nukleus.Application.Common.Persistence;
using Nukleus.Application.UserModule;

namespace Nukleus.Application.AccountModule
{
    public record AccountSingleDTO
    {
        public Guid Id { get; set; } = default!;
        public string Name { get; set; } = null!;
        public Guid? OwnerUserId { get; set; }

        public UserSingleDTO OwnerUser { get; set; }
        public List<UserSingleDTO> Users { get; set; }
    }

    // Used to handle unique FK constraints when neither exist.
    public record NewAccountDTO
    {
        // User Fields
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int? Age { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        // Account Fields
        public string AccountName { get; set; } = null!;
    }

    // Used to handle unique FK constraints when neither exist.
    public record NewAccountResponseDTO
    {
        public UserSingleDTO User { get; set; }
        public AccountSingleDTO Account { get; set; }
    }

    public record AccountSearchDTO
    {
        public List<FieldFilter>? Name { get; set; }
        public List<FieldFilter>? OwnerUserId { get; set; }
    }

    public record AccountSearchResultDTO
    {
        public Guid Id { get; set; } = default!;
        public string Name { get; set; } = null!;
        public Guid? OwnerUserId { get; set; }

        public UserSingleDTO OwnerUser { get; set; }
        public List<UserSingleDTO> Users { get; set; }
    }

    public record AccountAddDTO
    {
        public string Name { get; set; } = null!;
        public Guid? OwnerUserId { get; set; }
    }

    public record AccountPatchDTO
    {
        public string? Name { get; set; }
        public Guid? OwnerUserId { get; set; }
    }
}