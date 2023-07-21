using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Common;
using StreamMasterDomain.Entities;

using StreamMasterInfrastructure.Common;

using System.Reflection;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : DbContext, IDataProtectionKeyContext, IAppDbContext

{
    private readonly ILogger<AppDbContext> _logger;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IMemoryCache _memoryCache;
    private readonly Setting _setting;

    public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
    {
        _setting = FileUtil.GetSetting();

        DbPath = Path.Join(Constants.DataDirectory, "StreamMaster.db");
    }

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IMediator mediator,
        IMapper mapper,
        IMemoryCache memoryCache,
        ILogger<AppDbContext> logger
    ) : base(options)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _mapper = mapper;
        _mediator = mediator;
        _setting = FileUtil.GetSetting();

        DbPath = Path.Join(Constants.DataDirectory, "StreamMaster.db");
    }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public string DbPath { get; }    

    private Setting setting
    {
        get
        {
            return FileUtil.GetSetting();
        }
    }

    public override int SaveChanges()
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
