using StreamMaster.Application.SMChannels.Commands;

namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SyncChannelsRequest(int M3UFileId) : IRequest<APIResponse>;

internal class SyncChannelsRequestHandler(ILogger<SyncChannelsRequest> logger, ISender sender, IMessageService messageService, IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<SyncChannelsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SyncChannelsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFile(request.M3UFileId).ConfigureAwait(false);
            if (m3uFile == null)
            {
                await messageService.SendError("Sync Channels M3U Not Found");
                return APIResponse.NotFound;
            }

            if (!m3uFile.SyncChannels)
            {
                await messageService.SendError("Sync Channels not set");
                return APIResponse.ErrorWithMessage("Sync Channels not set");
            }

            IQueryable<SMStream> streams = Repository.SMStream.GetQuery(tracking: false).Where(a => a.M3UFileId == request.M3UFileId);
            IQueryable<SMChannel> existingSMChannels = Repository.SMChannel.GetQuery(tracking: false).Where(a => a.M3UFileId == request.M3UFileId);

            // Get the stream IDs
            List<string> streamIds = await streams.Select(s => s.Id).ToListAsync(cancellationToken).ConfigureAwait(false);
            List<string?> existingChannelStreamIds = await existingSMChannels.Select(c => c.StreamID).ToListAsync(cancellationToken).ConfigureAwait(false);

            List<string> exist = existingChannelStreamIds.OfType<string>().ToList();

            // Identify streams to be created (in streams but not in existingSMChannels)
            List<string> streamsToBeCreated = streamIds.Except(exist).ToList();

            // Identify streams to be deleted (in existingSMChannels but not in streams)
            IEnumerable<string?> streamsToBeDeleted = existingChannelStreamIds.Except(streamIds);

            //foreach (string? stream in streamsToBeCreated)
            //{
            //    await sender.Send(new CreateSMChannelFromStreamsRequest())
            //}

            if (streamsToBeCreated.Count != 0)
            {
                await sender.Send(new CreateSMChannelFromStreamsRequest(streamsToBeCreated, null, request.M3UFileId), cancellationToken).ConfigureAwait(false);
            }

            if (streamsToBeDeleted.Any())
            {
                List<int> smChannelIds = await Repository.SMChannel.GetQuery().Where(a => a.M3UFileId == request.M3UFileId && a.StreamID != null && streamsToBeDeleted.Contains(a.StreamID)).Select(a => a.Id).ToListAsync(cancellationToken: cancellationToken);

                await sender.Send(new DeleteSMChannelsRequest(smChannelIds), cancellationToken).ConfigureAwait(false);
            }

            if (streamsToBeCreated.Count != 0 || streamsToBeDeleted.Any())
            {
                await dataRefreshService.RefreshSMChannels();
            }

            await messageService.SendSuccess("Sync Channels for M3U '" + m3uFile.Name + "' successfully");
            return APIResponse.Success;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Sync Channels Error");
            await messageService.SendError(ex.Message, "Sync Channels M3U");
            return APIResponse.NotFound;
        }
    }
}
