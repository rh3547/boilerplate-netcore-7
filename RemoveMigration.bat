echo off
echo Removing last migration...
dotnet ef migrations remove --project Nukleus.Infrastructure --startup-project Nukleus.API --context NukleusDbContext
pause