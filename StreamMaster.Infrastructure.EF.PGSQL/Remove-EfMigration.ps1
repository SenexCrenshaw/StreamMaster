
    param (
        [Parameter(Mandatory=$true)]
        [string]$MigrationName
    )

    $command = "dotnet ef migrations add $MigrationName -c PGSQLRepositoryContext -o .\Migrations\Repository\"
    Invoke-Expression $command
