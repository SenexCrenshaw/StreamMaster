# Install-Package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
# Install-Package Microsoft.EntityFrameworkCore.Sqlite

# dotnet tool install --global dotnet-ef

#dotnet ef database drop -f
#rm -Recurse .\Persistence\Migrations\
dotnet ef migrations add InitialCreate -c LogDbContext -o .\Logging\Migrations

dotnet ef migrations add InitialCreate -c AppDbContext -o .\Persistence\Migrations

dotnet ef database update -c LogDbContext

dotnet ef database update -c AppDbContext
