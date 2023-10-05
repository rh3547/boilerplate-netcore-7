using Nukleus.Domain.Common;
using Nukleus.Domain.SeedWork;

namespace Nukleus.Domain.Entities
{
    public class User : Entity, IAggregateRoot
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public string? ResetPasswordToken {get; set;}
        public DateTime? ResetPasswordTokenExpiryTime {get; set;}
        public virtual Guid? AccountId { get; set; }

        public virtual Account? Account { get; set; }

        // Authentication Fields
        // https://stackoverflow.com/questions/3390105/best-way-to-store-hashed-passwords-and-salt-values-in-the-database-varchar-or
        // public Base64String PasswordSalt { get; set; } = null!;
         
        // public Base64String? JWTSalt { get; set; } = null!;
    }
}