using Nukleus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nukleus.Infrastructure.UserModule
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.FirstName);
            builder.Property(b => b.LastName);
            builder.Property(b => b.Age);
            builder.Property(b => b.Username).IsRequired();
            builder.Property(b => b.Email).IsRequired();
            builder.Property(b => b.Password).IsRequired();

            builder
                .HasOne(b => b.Account)
                .WithMany(b => b.Users)
                .HasForeignKey(b => b.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        }
    }
}