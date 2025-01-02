namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateSMChannelRequest(
    string Name,
    List<string>? SMStreamsIds,
    string? CommandProfileName,
     string? ClientUserAgent,
    int? ChannelNumber,
    int? TimeShift,
    string? Group,
    string? EPGId,
     string? StationId,
    string? Logo
    ) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class CreateSMChannelRequestHandler(ILogger<CreateSMChannelRequest> Logger, ISMWebSocketManager sMWebSocketManager, IImageDownloadQueue imageDownloadQueue, IMessageService messageService, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository)
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
            SMChannel smChannel = new()
            {
                Name = request.Name,
                ChannelNumber = request.ChannelNumber ?? 0,
                TimeShift = request.TimeShift ?? 0,
                Group = request.Group ?? "All",
                EPGId = request.EPGId ?? string.Empty,
                Logo = request.Logo ?? string.Empty,
                StationId = request.StationId ?? string.Empty,
                CommandProfileName = request.CommandProfileName ?? "Default",
                ClientUserAgent = request.ClientUserAgent
            };

            Repository.SMChannel.Create(smChannel);
            _ = await Repository.SaveAsync();

            if (request.SMStreamsIds != null)
            {
                int count = 0;
                foreach (string streamId in request.SMStreamsIds)
                {
                    await Repository.SMChannelStreamLink.CreateSMChannelStreamLink(smChannel.Id, streamId, count++).ConfigureAwait(false);
                }
                _ = await Repository.SaveAsync();

                //DataResponse<List<SMStreamDto>> streams = await Sender.Send(new UpdateStreamRanksRequest(smChannel.Id, request.SMStreamsIds), cancellationToken);
            }

            LogoInfo logoInfo = new(smChannel.Name, smChannel.Logo);
            imageDownloadQueue.EnqueueLogo(logoInfo);

            await dataRefreshService.RefreshAllSMChannels();
            await messageService.SendSuccess("Channel Added", $"Channel '{request.Name}' added successfully");
            await sMWebSocketManager.BroadcastReloadAsync();
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
