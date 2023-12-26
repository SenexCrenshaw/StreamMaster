using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using System.Collections.Concurrent;
using System.Web;

namespace StreamMaster.Application.Icons.Commands;

public class BuildIconsCacheFromVideoStreamRequest : IRequest<bool> { }

[LogExecutionTimeAspect]
public class BuildIconsCacheFromVideoStreamRequestHandler : BaseMediatorRequestHandler, IRequestHandler<BuildIconsCacheFromVideoStreamRequest, bool>
{

    public BuildIconsCacheFromVideoStreamRequestHandler(ILogger<BuildIconsCacheFromVideoStreamRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

    public Task<bool> Handle(BuildIconsCacheFromVideoStreamRequest command, CancellationToken cancellationToken)
    {
        return Task.FromResult( true);
        //IQueryable<VideoStream> streams = Repository.VideoStream.GetVideoStreamQuery()
        // .Where(a => a.User_Tvg_logo != null && a.User_Tvg_logo.Contains("://"))
        // .AsQueryable();

        //if (!streams.Any()) { return Task.FromResult(false); }

        //// For progress reporting
        //int totalCount = streams.Count();

        //ParallelOptions parallelOptions = new()
        //{
        //    CancellationToken = cancellationToken,
        //    MaxDegreeOfParallelism = Environment.ProcessorCount // or any number depending on how much parallelism you want
        //};

        ////var streamsList = await streams.ToListAsync();

        //ConcurrentBag<IconFileDto> toWrite = new();

        //_ = Parallel.ForEach(streams, parallelOptions, stream =>
        //{
        //    if (cancellationToken.IsCancellationRequested) { return; }

        //    string source = HttpUtility.UrlDecode(stream.Tvg_logo);

        //    IconFileDto icon = IconHelper.GetIcon(source, stream.User_Tvg_name, stream.M3UFileId, FileDefinitions.Icon);
        //    toWrite.Add(icon);
        //});


        //List<IconFileDto> icons = MemoryCache.Icons();
        //IEnumerable<IconFileDto> missingIcons = toWrite.Except(icons, new IconFileDtoComparer());
        //missingIcons = missingIcons.Distinct(new IconFileDtoComparer());
        //icons.AddRange(missingIcons);
        //for (int i = 0; i < icons.Count; i++)
        //{
        //    icons[i].Id = i;
        //}
        //MemoryCache.SetCache(icons);

        //return Task.FromResult(true);
    }
}
