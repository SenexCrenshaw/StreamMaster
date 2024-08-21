namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetVideoInfoRequest(string SMStreamId) : IRequest<DataResponse<VideoInfo?>>;

internal class GetVideoInfoRequestHandler(IVideoInfoService videoInfoService)
    : IRequestHandler<GetVideoInfoRequest, DataResponse<VideoInfo?>>
{
    public async Task<DataResponse<VideoInfo?>> Handle(GetVideoInfoRequest request, CancellationToken cancellationToken)
    {
        VideoInfo? ret = videoInfoService.GetVideoInfo(request.SMStreamId);
        return await Task.FromResult(DataResponse<VideoInfo?>.Success(ret));
    }
}
