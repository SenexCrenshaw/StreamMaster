using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;

namespace StreamMaster.Infrastructure.EF.PSQL.Logging;

public class LogDbContextInitialiser
{
    private readonly LogDbContext _context;
    private readonly ILogger<LogDbContextInitialiser> _logger;

    public LogDbContextInitialiser(
        ILogger<LogDbContextInitialiser> logger,
        LogDbContext context
        )
    {
        _context = context;
        _logger = logger;

    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlite())
            {
                await _context.Database.MigrateAsync().ConfigureAwait(false);
                _context.RemoveRange(_context.LogEntries);
                await _context.SaveChangesAsync().ConfigureAwait(false);
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
