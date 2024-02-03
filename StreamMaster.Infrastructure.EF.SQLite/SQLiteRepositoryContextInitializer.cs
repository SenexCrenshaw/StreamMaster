using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace StreamMaster.Infrastructure.EF.SQLite;

public class SQLiteRepositoryContextInitializer
{
    private readonly SQLiteRepositoryContext _context;
    private readonly ILogger<SQLiteRepositoryContextInitializer> _logger;

    public SQLiteRepositoryContextInitializer(
        ILogger<SQLiteRepositoryContextInitializer> logger,
        SQLiteRepositoryContext context
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
                if (!_context.StreamGroups.Any(a => a.Name == "ALL"))
                {
                    _context.Add(new StreamGroup { Name = "ALL", IsReadOnly = true });
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }

                if (!_context.ChannelGroups.Any(a => a.Name == "(None)"))
                {
                    _context.Add(new ChannelGroup { Name = "(None)", IsReadOnly = true });
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                //_context.Database.ExecuteSqlRaw("PRAGMA journal_mode = 'delete';");
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

    public void MigrateData()
    {

    }
}