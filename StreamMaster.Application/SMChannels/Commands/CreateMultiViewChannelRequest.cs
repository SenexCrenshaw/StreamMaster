namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateMultiViewChannelRequest(
    string Name,
     List<int>? SMSChannelIds,
     string? StreamGroup,
    int? ChannelNumber,
    string? Group,
    string? EPGId,
    string? Logo
    ) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class CreateMultiViewChannelRequestHandler(ILogger<CreateMultiViewChannelRequest> Logger, IImageDownloadQueue imageDownloadQueue, IMessageService messageService, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository)
    : IRequestHandler<CreateMultiViewChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateMultiViewChannelRequest request, CancellationToken cancellationToken)
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
                TimeShift = 0,
                Group = request.Group ?? "All",
                EPGId = request.EPGId ?? string.Empty,
                Logo = request.Logo ?? string.Empty,
                StationId = string.Empty,
                CommandProfileName = "Default",
                SMChannelType = SMChannelTypeEnum.MultiView
            };

            Repository.SMChannel.Create(smChannel);
            _ = await Repository.SaveAsync();

            if (request.SMSChannelIds != null)
            {
                int count = 0;
                foreach (int smSChannelId in request.SMSChannelIds)
                {
                    await Repository.SMChannelChannelLink.CreateSMChannelChannelLink(smChannel.Id, smSChannelId, count++).ConfigureAwait(false);
                }
                _ = await Repository.SaveAsync();

                //DataResponse<List<SMStreamDto>> streams = await Sender.Send(new UpdateStreamRanksRequest(smChannel.Id, request.SMStreamsIds), cancellationToken);
            }

            //NameLogo NameLogo = new NameLogo(smChannel);
            //imageDownloadQueue.EnqueueNameLogo(NameLogo);

            await dataRefreshService.RefreshAllSMChannels();
            await messageService.SendSuccess("Channel Added", $"Channel '{request.Name}' added successfully");
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
