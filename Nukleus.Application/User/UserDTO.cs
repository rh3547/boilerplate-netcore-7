using Nukleus.Application.AccountModule;
using Nukleus.Application.Common.Persistence;
using Nukleus.Domain.Entities;

namespace Nukleus.Application.UserModule
{
    // 'required' keyword effectively allows us to not use constructors,
    // while still requiring certain fields be used when instantiated.
    public record UserSingleDTO
    {
        public Guid Id { get; set; } = default!; // Guid is not nullable, it can be set to default, we want to assure it is not.
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public Guid AccountId { get; set; }

        public AccountSingleDTO Account { get; set; }
    }

    public record UserSearchDTO
    {
        public List<FieldFilter>? FirstName { get; set; }
        public List<FieldFilter>? LastName { get; set; }
        public List<FieldFilter>? Age { get; set; }
        public List<FieldFilter>? Email { get; set; }
        public List<FieldFilter>? Username { get; set; }
        public List<FieldFilter>? AccountId { get; set; }
    }

    public record UserSearchResultDTO
    {
        public Guid Id { get; set; } = default!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public Guid AccountId { get; set; }

        public AccountSingleDTO Account { get; set; }
    }

    public record UserAddDTO
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int? Age { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Guid? AccountId { get; set; }
    }

    public record UserPatchDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public Guid? AccountId { get; set; }
    }

    public record AuthenticateRequestDTO
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}