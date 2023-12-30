using System.Web;

namespace StreamMaster.Application.Icons.Commands;

public class BuildIconsCacheFromVideoStreamRequest : IRequest<bool> { }

[LogExecutionTimeAspect]
public class BuildIconsCacheFromVideoStreamRequestHandler(ILogger<BuildIconsCacheFromVideoStreamRequest> logger, IIconService iconService, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<BuildIconsCacheFromVideoStreamRequest, bool>
{
    public Task<bool> Handle(BuildIconsCacheFromVideoStreamRequest command, CancellationToken cancellationToken)
    {

        IQueryable<VideoStream> streams = Repository.VideoStream.GetVideoStreamQuery()
         .Where(a => a.User_Tvg_logo != null && a.User_Tvg_logo.Contains("://"))
         .AsQueryable();

        if (!streams.Any()) { return Task.FromResult(false); }

        int totalCount = streams.Count();

        ParallelOptions parallelOptions = new()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        _ = Parallel.ForEach(streams, parallelOptions, stream =>
        {
            if (cancellationToken.IsCancellationRequested) { return; }

            string source = HttpUtility.UrlDecode(stream.Tvg_logo);

            IconFileDto icon = IconHelper.GetIcon(source, stream.User_Tvg_name, stream.M3UFileId, FileDefinitions.Icon);
            iconService.AddIcon(icon);
        });
        //iconService.SetIndexes();

        return Task.FromResult(true);
    }
}
