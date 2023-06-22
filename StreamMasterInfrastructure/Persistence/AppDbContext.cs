using MediatR;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Common;
using StreamMasterDomain.Entities;

using StreamMasterInfrastructure.Common;

using System.Reflection;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : DbContext, IDataProtectionKeyContext,IAppDbContext

{
    private readonly ILogger<AppDbContext> _logger;
    private readonly IMediator _mediator;

    private readonly Setting _setting;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public AppDbContext(
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
       DbContextOptions<AppDbContext> options
        )
       : base(options)
    {
        _setting = FileUtil.GetSetting();
        //FileUtil.SetupDirectories();

        DbPath = Path.Join(Constants.DataDirectory, _setting.DatabaseName ?? "StreamMaster.db");
    }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IMediator mediator,
        ILogger<AppDbContext> logger
    // AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor
    ) : base(options)
    {
        _logger = logger;

        _mediator = mediator;
        _setting = FileUtil.GetSetting();

        DbPath = Path.Join(Constants.DataDirectory, _setting.DatabaseName ?? "StreamMaster.db");

        //_auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public string DbPath { get; }

   
    public int SaveChanges()
    {
        _ = _mediator.DispatchDomainEvents(this).ConfigureAwait(false);
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this).ConfigureAwait(false);

        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        FileUtil.SetupDirectories();

        _ = options.UseSqlite(
            $"Data Source={DbPath}",
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            );
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        _ = builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        _ = builder.Entity<EPGFile>().ToTable("EPGFiles");
        _ = builder.Entity<M3UFile>().ToTable("M3UFiles");

        base.OnModelCreating(builder);
    }
}
