
    param (
        [Parameter(Mandatory=$true)]
        [string]$MigrationName
    )

    dotnet tool update --global dotnet-ef


    # Replace spaces with underscores
$MigrationName = $MigrationName -replace ' ', '_'

# Remove any invalid characters
$MigrationName = $MigrationName -replace '[^a-zA-Z0-9_]', ''

# Log the migration name
Write-Host "Removing migration $MigrationName to PGSQLRepositoryContext"

    $command = "dotnet ef migrations add $MigrationName -c PGSQLRepositoryContext -o .\Migrations\Repository\"
    Invoke-Expression $command
