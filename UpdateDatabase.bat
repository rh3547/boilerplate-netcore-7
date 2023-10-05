echo off
echo Updating database...
dotnet ef database update --project Nukleus.Infrastructure --startup-project Nukleus.API --context NukleusDbContext
pause