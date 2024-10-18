
  
# Log the migration name
Write-Host "Removing last migration"

    $command = "dotnet ef migrations remove -c PGSQLRepositoryContext" 
    Invoke-Expression $command
