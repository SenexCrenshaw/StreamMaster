# Install-Package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
# Install-Package Microsoft.EntityFrameworkCore.Sqlite

# dotnet tool install --global dotnet-ef

#dotnet ef database drop -f

# dotnet ef database drop -f
# dotnet ef database drop -f -c AppDbContext

#rm -Recurse .\Persistence\Migrations\
dotnet ef migrations add InitialCreate -c RepositoryContext -o .\Migrations\Repository\

dotnet ef migrations add InitialCreate -c LogDbContext -o .\Migrations\Logging\

dotnet ef migrations add InitialCreate -c AppDbContext -o .\Migrations\Persistence\

dotnet ef database update -c RepositoryContext

dotnet ef database update -c LogDbContext

dotnet ef database update -c AppDbContext
