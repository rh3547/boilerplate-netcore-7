using Nukleus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nukleus.Infrastructure.AccountModule
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {

        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(b => b.Id); // By convention, non-composite primary keys of type short, int, long, or Guid are set up to have values generated for inserted entities if a value isn't provided by the application.
            builder.Property(b => b.Name).IsRequired();

            builder
                .HasOne(b => b.OwnerUser)
                .WithOne()
                .HasForeignKey<Account>(b => b.OwnerUserId);

            builder
                .HasMany(b => b.Users)
                .WithOne(b => b.Account)
                .HasForeignKey(b => b.AccountId);
        }
    }
}