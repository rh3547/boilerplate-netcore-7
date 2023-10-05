# Entity Generator
Use this tool to enter information about the backend entity you would like to create.
All backend files will be generated automatically.
NOTE: This will not add the DbSet to Nukleus.Infrastructure/Common/Persistence/NukleusDbContext.cs.
      After generating files with this tool, add the DbSet to the file like so: `public DbSet<EntityName> EntityName { get; set; } = null!;`
NOTE: This will not register dependencies in Nukleus.Infrastructure/RegisterDependenciesDI.cs.
      After generating files with this tool, add the dependency registrations to the file like so:
      - `services.AddScoped<IEntityNameRepository, EntityNameRepository>();`
      - `services.AddScoped<IEntityNameService, EntityNameService>();`

## Setup:
1. Run `npm install`.
2. Modify the config.json file with the correct file paths if necessary. By default the paths should be relative.

## Run:
1. Run `.\RunEntityGenerator.bat` (Type "Run" and auto-complete with tab). Or, run: `npm start`.

## Usage Documentation
- The generator will require you to enter a name for the entity you are generating. Specifying entity inheritance is not currently supported. That will have to be done manually after initial generation.
- You may enter any number of fields for the entity by providing a type, name, and indicating whether it is nullable.
      - Any row of field inputs that are blank will be ignored. aka you do not need to remove the blank row at the end of the fields list when generating.
      - Be sure to enter data types and names exactly as you intend, the system does not modify casing.
            - Remember, C# standard is to have capital field names but lowercase primitive data types. i.e. "string Name", string being the field type and Name being the field name. Field types referencing other non-primitive entities should be capitalized to match the entity name exactly. i.e. "User User".
- Fields named "Id" or "id" will automatically be considered the id field and will be generated differently in certain circumstances.
- Fields with a non-primitive type will automatically be omitted from Add,Patch, and Search DTOs.
- Relations in the entity framework config will be infered from the data type entered for each field.
      - If you wish to add a HasMany relation, the data type should be: List<EntityName>, where EntityName is the name of the entity for the list.
      - If you wish to add a HasOne relation, the data type should be: EntityName, where EntityName is the name of the entity for the field.
      - You can specify the foreign key and the related entity's navigation object with the following syntax:
            - For HasOne relations: add the following after the field type: {ForeignKeyName}.
                  - Example: User{UserId}. In the case of a HasOne relation, the foreign key is referencing a field on the current entity itself. This example would expect the "UserId" field to be on the entity you are creating.
            - For HasMany relations: add the following after the field type: {ForeignKeyName,RelatedEntityObject}.
                  - Example: List<User>{AccountId,Account}. In the case of a HasMany relation, the foreign key is referencing a field on the related entity. This example would expect the "AccountId" and "Account" fields to be on the related entity.
            - NOTE: Do not add any spaces in the braces when declaring relation parameters. i.e. Don't do this: {AccountId, Account}

### Example:
EntityGenerator Input:
Entity Name: Account
Field: Type: Guid, Name: Id, Nullable: False
Field: Type: string, Name: Name, Nullable: True
Field: Type: Guid, Name: OwnerUserId, Nullable: True
Field: Type: User{OwnerUserId}, Name: OwnerUser, Nullable: True
Field: Type: List<User>{AccountId,Account}, Name: Users, Nullable: True

Will generate the following:

Account.cs:
using Nukleus.Domain.SeedWork;
namespace Nukleus.Domain.Entities
{
    public class Account : IAggregateRoot
    {
        public Guid Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Guid? OwnerUserId { get; set; }
        public User? OwnerUser { get; set; }
        public List<User>? Users { get; set; }
    }
}

AccountConfiguration.cs:
using Nukleus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Nukleus.Infrastructure.Account
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Name).IsRequired();
            builder.Property(b => b.OwnerUserId);
            builder.HasOne(b => b.OwnerUser).WithOne().HasForeignKey<Account>(b => b.OwnerUserId);
            builder.HasMany(b => b.Users).WithOne(b => b.Account).HasForeignKey(b => b.AccountId);
        }
    }
}

etc...
