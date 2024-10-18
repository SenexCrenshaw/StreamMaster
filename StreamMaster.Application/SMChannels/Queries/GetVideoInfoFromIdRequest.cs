

//namespace StreamMaster.Application.SMChannels.Queries;

//[SMAPI]
//[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
//public record GetVideoInfoFromIdRequest(int SMChannelId) : IRequest<DataResponse<VideoInfo>>;

//internal class GetVideoInfoFromIdRequestHandler(IChannelManager channelManager)
//    : IRequestHandler<GetVideoInfoFromIdRequest, DataResponse<VideoInfo>>
//{
//    public async Task<DataResponse<VideoInfo>> Handle(GetVideoInfoFromIdRequest request, CancellationToken cancellationToken)
//    {
//        var ret = channelManager.GetVideoInfo(request.SMChannelId);
//        return DataResponse<VideoInfo>.Success(ret);
//    }
//}
