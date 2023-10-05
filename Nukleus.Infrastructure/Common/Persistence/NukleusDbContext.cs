using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nukleus.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Nukleus.Infrastructure.Common.Persistence
{
    public class NukleusDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;
        public NukleusDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<User> User { get; set; } = null!;
        public DbSet<Account> Account { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("WebApiDatabase"));
        }
    }
}