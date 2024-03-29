using StreamMaster.SchedulesDirect.Helpers;

using System.Web;

namespace StreamMaster.Application.Icons.Commands;

public class BuildIconsCacheFromVideoStreamRequest : IRequest<bool> { }

[LogExecutionTimeAspect]
public class BuildIconsCacheFromVideoStreamRequestHandler(ILogger<BuildIconsCacheFromVideoStreamRequest> logger, IIconService iconService, IRepositoryWrapper Repository)
    : IRequestHandler<BuildIconsCacheFromVideoStreamRequest, bool>
{
    public Task<bool> Handle(BuildIconsCacheFromVideoStreamRequest command, CancellationToken cancellationToken)
    {

        IQueryable<SMStreamDto> streams = Repository.SMStream.GetSMStreams()
         //        .Where(a => a.User_Tvg_logo != null && EF.Functions.ILike(a.User_Tvg_logo, "://"))
         .Where(a => a.Logo != null && a.Logo.Contains("://"))
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

            string source = HttpUtility.UrlDecode(stream.Logo);

            IconFileDto icon = IconHelper.GetIcon(source, stream.Name, stream.M3UFileId, FileDefinitions.Icon);
            iconService.AddIcon(icon);
        });

        return Task.FromResult(true);
    }
}
