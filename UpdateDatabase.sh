#!/bin/bash

echo "Updating database..."
dotnet ef database update --project Nukleus.Infrastructure --startup-project Nukleus.API --context NukleusDbContext

read -p "Press any key to continue..." key
