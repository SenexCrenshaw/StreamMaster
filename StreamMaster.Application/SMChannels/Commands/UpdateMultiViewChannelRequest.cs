namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateMultiViewChannelRequest(int Id, string? Name, List<int>? SMChannelIds, int? ChannelNumber, string? Group, string? EPGId, string? Logo)
    : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class UpdateMultiViewChannelRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<UpdateMultiViewChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateMultiViewChannelRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<FieldData> ret = [];

            SMChannel? smChannel = Repository.SMChannel.GetSMChannel(request.Id);
            if (smChannel == null)
            {
                return APIResponse.NotFound;
            }

            if (!string.IsNullOrEmpty(request.Name) && request.Name != smChannel.Name)
            {
                smChannel.Name = request.Name;
                ret.Add(new FieldData(() => smChannel.Name));
            }

            if (!string.IsNullOrEmpty(request.Group) && request.Group != smChannel.Group)
            {
                smChannel.Group = request.Group;
                ret.Add(new FieldData(() => smChannel.Group));
            }

            if (!string.IsNullOrEmpty(request.EPGId) && request.EPGId != smChannel.EPGId)
            {
                smChannel.EPGId = request.EPGId;
                ret.Add(new FieldData(() => smChannel.EPGId));
            }

            if (!string.IsNullOrEmpty(request.Logo) && request.Logo != smChannel.Logo)
            {
                smChannel.Logo = request.Logo;
                ret.Add(new FieldData(() => smChannel.Logo));
            }

            if (request.ChannelNumber.HasValue && request.ChannelNumber.Value != smChannel.ChannelNumber)
            {
                smChannel.ChannelNumber = request.ChannelNumber.Value;
                ret.Add(new FieldData(() => smChannel.ChannelNumber));
            }

            if (ret.Count > 0)
            {
                Repository.SMChannel.Update(smChannel);
                _ = await Repository.SaveAsync().ConfigureAwait(false);
                await dataRefreshService.RefreshSMChannels().ConfigureAwait(false);
            }

            return APIResponse.Success;
        }
        catch (Exception ex)
        {
            return APIResponse.ErrorWithMessage(ex, "Failed M3U update");
        }
    }
}
