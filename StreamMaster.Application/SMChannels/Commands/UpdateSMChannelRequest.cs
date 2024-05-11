﻿namespace StreamMaster.Application.SMChannels.Commands;


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateSMChannelRequest(int Id, string? Name, List<string>? SMStreamsIds, StreamingProxyTypes? StreamingProxyType, int? ChannelNumber, int? TimeShift, string? Group, string? EPGId, string? Logo, VideoStreamHandlers? VideoStreamHandler)
    : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class UpdateSMChannelRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
    : IRequestHandler<UpdateSMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateSMChannelRequest request, CancellationToken cancellationToken)
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

            if (smChannel.StreamingProxyType != request.StreamingProxyType)
            {
                smChannel.StreamingProxyType = request.StreamingProxyType.Value;
                ret.Add(new FieldData(() => smChannel.StreamingProxyType));
            }

            if (request.ChannelNumber.HasValue && request.ChannelNumber.Value != smChannel.ChannelNumber)
            {
                smChannel.ChannelNumber = request.ChannelNumber.Value;
                ret.Add(new FieldData(() => smChannel.ChannelNumber));
            }


            if (request.TimeShift.HasValue && request.TimeShift.Value != smChannel.TimeShift)
            {
                smChannel.TimeShift = request.TimeShift.Value;
                ret.Add(new FieldData(() => smChannel.TimeShift));
            }

            if (ret.Count > 0)
            {
                Repository.SMChannel.Update(smChannel);
                await Repository.SaveAsync().ConfigureAwait(false);
                await hubContext.Clients.All.SetField(ret).ConfigureAwait(false);
                await hubContext.Clients.All.DataRefresh("GetSMChannel");

            }

            return APIResponse.Success;
        }
        catch (Exception ex)
        {

            return APIResponse.ErrorWithMessage(ex, $"Failed M3U update");
        }

    }



}