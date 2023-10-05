echo off
set migrationName=%1
echo Building and adding migration '%migrationName%'...
dotnet ef migrations add %migrationName% -o Common/Persistence/Migrations --project Nukleus.Infrastructure --startup-project Nukleus.API --context NukleusDbContext
pause