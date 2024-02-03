
# dotnet tool install --global dotnet-ef

#dotnet ef database drop -f

# dotnet ef database drop -f
# dotnet ef database drop -f -c AppDbContext

#rm -Recurse .\Persistence\Migrations\
dotnet ef migrations add InitialCreate -c RepositoryContext -o .\Migrations\Repository\

dotnet ef migrations add InitialCreate -c LogDbContext -o .\Migrations\Logging\

dotnet ef database update -c RepositoryContext

dotnet ef database update -c LogDbContext

dotnet tool update --global dotnet-ef


//dotnet ef migrations remove -c RepositoryContext
//dotnet ef database update -c RepositoryContext 20231229192654_SystemKeyValues