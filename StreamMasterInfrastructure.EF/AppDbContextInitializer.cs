using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.Repository;

using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructure.EF;

public class AppDbContextInitialiser
{
    private readonly AppDbContext _context;
    private readonly ILogger<AppDbContextInitialiser> _logger;

    public AppDbContextInitialiser(
        ILogger<AppDbContextInitialiser> logger,
        AppDbContext context
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
                if (!_context.ChannelGroups.Any(a => a.Name == "(None)"))
                {
                    _context.Add(new ChannelGroup { Name = "(None)", IsReadOnly = false, Rank = 1 });
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
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
