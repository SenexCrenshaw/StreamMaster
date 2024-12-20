namespace StreamMaster.Application.SMStreams.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateSMStreamRequest(
    string SMStreamId,
    string? Name,
    int? ChannelNumber,
    string? Group,
    string? Logo,
       string? EPGID,
        string? CommandProfileName,
    string Url
    ) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class UpdateSMStreamHandler(ILogger<UpdateSMStreamRequest> Logger, IMessageService messageService, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository)
    : IRequestHandler<UpdateSMStreamRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateSMStreamRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.SMStreamId))
        {
            return APIResponse.NotFound;
        }

        SMStream? smStream = await Repository.SMStream.FirstOrDefaultAsync(a => a.Id == request.SMStreamId, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (smStream == null)
        {
            return APIResponse.NotFound;
        }

        List<FieldData> ret = [];
        try
        {
            if (!string.IsNullOrEmpty(request.Name) && smStream.Name != request.Name)
            {
                smStream.Name = request.Name;
                ret.Add(new FieldData(() => smStream.Name));
            }
            if (request.ChannelNumber.HasValue && smStream.ChannelNumber != request.ChannelNumber.Value)
            {
                smStream.ChannelNumber = request.ChannelNumber.Value;
                ret.Add(new FieldData(() => smStream.ChannelNumber));
            }

            if (!string.IsNullOrEmpty(request.Group) && smStream.Group != request.Group)
            {
                smStream.Group = request.Group;
                ret.Add(new FieldData(() => smStream.Group));
            }

            if (!string.IsNullOrEmpty(request.EPGID) && smStream.EPGID != request.EPGID)
            {
                smStream.EPGID = request.EPGID;
                ret.Add(new FieldData(() => smStream.EPGID));
            }

            if (!string.IsNullOrEmpty(request.Logo) && smStream.Logo != request.Logo)
            {
                smStream.Logo = request.Logo;
                ret.Add(new FieldData(() => smStream.Logo));
            }

            if (!string.IsNullOrEmpty(request.CommandProfileName) && smStream.CommandProfileName != request.CommandProfileName)
            {
                smStream.CommandProfileName = request.CommandProfileName;
                ret.Add(new FieldData(() => smStream.CommandProfileName));
            }

            if (!string.IsNullOrEmpty(request.Url) && smStream.Url != request.Url)
            {
                smStream.Url = request.Url;
                ret.Add(new FieldData(() => smStream.Url));
            }

            if (ret.Count > 0)
            {
                Repository.SMStream.Update(smStream);
                await Repository.SaveAsync().ConfigureAwait(false);
                await dataRefreshService.ClearByTag(SMStream.APIName, "IsHidden").ConfigureAwait(false);
                await dataRefreshService.SetField(ret).ConfigureAwait(false);
            }
            //await dataRefreshService.RefreshSMStreams();
            await messageService.SendSuccess("Stream Updated", $"Stream '{smStream.Name}' successfully");
            return APIResponse.Success;
        }
        catch (Exception exception)
        {
            await messageService.SendError("Exception adding Stream", exception.Message);
            Logger.LogCritical("Exception adding Stream '{exception}'", exception);
        }
        return APIResponse.NotFound;
    }
}
