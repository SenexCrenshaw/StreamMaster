using StreamMaster.Application.SMChannels.Commands;

namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SyncChannelsRequest(int M3UFileId, string? Group) : IRequest<APIResponse>;

internal class SyncChannelsRequestHandler(ILogger<SyncChannelsRequest> logger, ISender sender, IMessageService messageService, IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<SyncChannelsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SyncChannelsRequest request, CancellationToken cancellationToken)
    {
        try
        {

            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFileAsync(request.M3UFileId).ConfigureAwait(false);
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

            int? sgId = null; ;
            if (m3uFile.DefaultStreamGroupName != null)
            {
                StreamGroupDto? sg = await Repository.StreamGroup.GetStreamGroupByName(m3uFile.DefaultStreamGroupName);
                if (sg != null)
                {
                    sgId = sg.Id;
                }
            }

            IQueryable<SMStream> streams = Repository.SMStream.GetQuery().Where(a => a.M3UFileId == request.M3UFileId);
            IQueryable<SMChannel> existingSMChannels = Repository.SMChannel.GetQuery().Where(a => a.M3UFileId == request.M3UFileId);

            // Get the stream IDs as strings
            List<string> streamIds = await streams.Select(s => s.Id).ToListAsync(cancellationToken).ConfigureAwait(false);
            List<string> existingChannelStreamIds = await existingSMChannels.Select(c => c.BaseStreamID).ToListAsync(cancellationToken).ConfigureAwait(false);

            // Filter out null values and convert to List<string>
            List<string> existingChannelStreamIdsNonNull = existingChannelStreamIds.OfType<string>().ToList();

            // Identify streams that exist both in streams and in existingSMChannels (intersection)
            List<string> existingStreamsIDsInDb = streamIds.Intersect(existingChannelStreamIdsNonNull).ToList();
            List<SMStream> existingStreamsInDb = [.. streams.Where(a => existingStreamsIDsInDb.Contains(a.Id))];

            // Identify streams to be created (in streams but not in existingSMChannels)
            List<string> streamsToBeCreated = streamIds.Except(existingChannelStreamIdsNonNull).ToList();

            // Identify streams to be deleted (in existingSMChannels but not in streams)
            List<string> streamsToBeDeleted = existingChannelStreamIdsNonNull.Except(streamIds).ToList();

            if (streamsToBeCreated.Count != 0)
            {
                APIResponse res = await Repository.SMChannel.CreateSMChannelsFromStreams(streamsToBeCreated, sgId);
                //await sender.Send(new CreateSMChannelFromStreamsRequest(streamsToBeCreated, null, request.M3UFileId), cancellationToken).ConfigureAwait(false);
            }

            if (streamsToBeDeleted.Count != 0)
            {
                List<int> smChannelIds = await Repository.SMChannel.GetQuery().Where(a => a.M3UFileId == request.M3UFileId && a.BaseStreamID != null && streamsToBeDeleted.Contains(a.BaseStreamID)).Select(a => a.Id).ToListAsync(cancellationToken: cancellationToken);

                _ = await sender.Send(new DeleteSMChannelsRequest(smChannelIds), cancellationToken).ConfigureAwait(false);
            }
            bool changed = false;

            if (streamsToBeCreated.Count != 0 || streamsToBeDeleted.Count != 0 || changed)
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
