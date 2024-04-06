using StreamMaster.SchedulesDirect.Helpers;

using System.Web;

namespace StreamMaster.Application.Icons.Commands;

public class BuildIconsCacheFromVideoStreamRequest : IRequest<DataResponse<bool>> { }

[LogExecutionTimeAspect]
public class BuildIconsCacheFromVideoStreamRequestHandler(IIconService iconService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IRepositoryWrapper Repository)
    : IRequestHandler<BuildIconsCacheFromVideoStreamRequest, DataResponse<bool>>
{
    public async Task<DataResponse<bool>> Handle(BuildIconsCacheFromVideoStreamRequest command, CancellationToken cancellationToken)
    {

        IQueryable<SMStreamDto> streams = Repository.SMStream.GetSMStreams()
         //        .Where(a => a.User_Tvg_logo != null && EF.Functions.ILike(a.User_Tvg_logo, "://"))
         .Where(a => a.Logo != null && a.Logo.Contains("://"))
         .AsQueryable();

        if (!streams.Any()) { return DataResponse.False; }

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
        await hubContext.Clients.All.DataRefresh("IconFileDto").ConfigureAwait(false);
        return DataResponse.True;
    }
}
