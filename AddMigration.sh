#!/bin/bash

migrationName="$1"

echo "Building and adding migration '${migrationName}'..."
dotnet ef migrations add "$migrationName" -o Common/Persistence/Migrations --project Nukleus.Infrastructure --startup-project Nukleus.API --context NukleusDbContext

read -p "Press any key to continue..." key
