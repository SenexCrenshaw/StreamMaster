using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;

namespace StreamMasterInfrastructure.EF;

public class RepositoryContextInitializer
{
    private readonly RepositoryContext _context;
    private readonly ILogger<RepositoryContextInitializer> _logger;

    public RepositoryContextInitializer(
        ILogger<RepositoryContextInitializer> logger,
        RepositoryContext context
        )
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlite())
            {
                await _context.Database.MigrateAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }


    public void TrySeed()
    {
        FileUtil.SetupDirectories();

    }
}
