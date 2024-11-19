using System.Collections.Concurrent;

namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetVideoInfosRequest() : IRequest<DataResponse<List<VideoInfoDto>>>;

internal class GetVideoInfosRequestHandler(IVideoInfoService videoInfoService)
    : IRequestHandler<GetVideoInfosRequest, DataResponse<List<VideoInfoDto>>>
{
    public Task<DataResponse<List<VideoInfoDto>>> Handle(GetVideoInfosRequest request, CancellationToken cancellationToken)
    {
        ConcurrentDictionary<string, VideoInfo> infos = videoInfoService.VideoInfos;
        List<VideoInfoDto> ret = [];
        foreach (KeyValuePair<string, VideoInfo> info in infos)
        {
            ret.Add(new VideoInfoDto(info));
        }

        return Task.FromResult(DataResponse<List<VideoInfoDto>>.Success(ret));
    }
}
