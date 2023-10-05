#!/bin/bash

echo "Removing last migration..."
dotnet ef migrations remove --project Nukleus.Infrastructure --startup-project Nukleus.API --context NukleusDbContext

read -p "Press any key to continue..." key
