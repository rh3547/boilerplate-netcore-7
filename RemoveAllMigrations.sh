#!/bin/bash

echo "Removing all migrations..."
dotnet ef database update 0 --project Nukleus.Infrastructure --startup-project Nukleus.API --context NukleusDbContext
dotnet ef migrations remove --project Nukleus.Infrastructure --startup-project Nukleus.API --context NukleusDbContext

read -p "Press any key to continue..." key
