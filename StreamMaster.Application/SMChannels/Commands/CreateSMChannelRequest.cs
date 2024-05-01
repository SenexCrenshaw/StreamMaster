namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateSMChannelRequest(
    string Name,
    List<string>? SMStreamsIds,
    StreamingProxyTypes? StreamingProxyType,
    int? ChannelNumber,
    int? TimeShift,
    string? Group,
    string? EPGId,
    string? Logo,
    VideoStreamHandlers? VideoStreamHandler

    ) : IRequest<APIResponse>
{ }

[LogExecutionTimeAspect]
public class CreateSMChannelRequestHandler(ILogger<CreateSMChannelRequest> Logger, IMessageService messageService, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository)
    : IRequestHandler<CreateSMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateSMChannelRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return APIResponse.NotFound;
        }

        try
        {
            var smChannel = new SMChannel
            {
                Name = request.Name,
                ChannelNumber = request.ChannelNumber ?? 0,
                TimeShift = request.TimeShift ?? 0,
                Group = request.Group ?? "All",
                EPGId = request.EPGId ?? string.Empty,
                Logo = request.Logo ?? string.Empty,
                VideoStreamHandler = request.VideoStreamHandler ?? VideoStreamHandlers.SystemDefault,
                StreamingProxyType = request.StreamingProxyType ?? StreamingProxyTypes.SystemDefault
            };

            Repository.SMChannel.Create(smChannel);

            await dataRefreshService.RefreshAllSMChannels();
            return APIResponse.Success;
        }
        catch (Exception exception)
        {
            await messageService.SendError("Exception adding Channel", exception.Message);
            Logger.LogCritical("Exception adding Channel '{exception}'", exception);
        }
        return APIResponse.NotFound;
    }
}