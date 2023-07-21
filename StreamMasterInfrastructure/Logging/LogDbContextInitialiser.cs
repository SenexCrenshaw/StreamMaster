using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;

namespace StreamMasterInfrastructure.Logging;

public class LogDbContextInitialiser
{
    private readonly LogDbContext _context;
    private readonly ILogger<LogDbContextInitialiser> _logger;

    public LogDbContextInitialiser(
        ILogger<LogDbContextInitialiser> logger,
        LogDbContext context
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
                _context.RemoveRange(_context.LogEntries);
                await _context.SaveChangesAsync().ConfigureAwait(false);
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
