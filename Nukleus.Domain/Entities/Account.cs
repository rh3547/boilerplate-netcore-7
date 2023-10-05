using Nukleus.Domain.SeedWork;

namespace Nukleus.Domain.Entities
{
    public class Account : Entity, IAggregateRoot
    {
        public string Name { get; set; } = null!;
        public Guid OwnerUserId { get; set; }

        public virtual User OwnerUser { get; set; }
        public virtual List<User> Users { get; set; }
    }
}