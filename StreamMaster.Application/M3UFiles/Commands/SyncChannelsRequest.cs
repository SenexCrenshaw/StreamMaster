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

            IQueryable<SMStream> streams = Repository.SMStream.GetQuery().Where(a => a.M3UFileId == request.M3UFileId);
            IQueryable<SMChannel> existingSMChannels = Repository.SMChannel.GetQuery().Where(a => a.M3UFileId == request.M3UFileId);

            // Get the stream IDs as strings
            List<string> streamIds = await streams.Select(s => s.Id).ToListAsync(cancellationToken).ConfigureAwait(false);
            List<string?> existingChannelStreamIds = await existingSMChannels.Select(c => c.StreamID).ToListAsync(cancellationToken).ConfigureAwait(false);

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
                APIResponse res = await Repository.SMChannel.CreateSMChannelsFromStreams(streamsToBeCreated, null, m3uFile.Id);
                //await sender.Send(new CreateSMChannelFromStreamsRequest(streamsToBeCreated, null, request.M3UFileId), cancellationToken).ConfigureAwait(false);
            }

            if (streamsToBeDeleted.Count != 0)
            {
                List<int> smChannelIds = await Repository.SMChannel.GetQuery().Where(a => a.M3UFileId == request.M3UFileId && a.StreamID != null && streamsToBeDeleted.Contains(a.StreamID)).Select(a => a.Id).ToListAsync(cancellationToken: cancellationToken);

                _ = await sender.Send(new DeleteSMChannelsRequest(smChannelIds), cancellationToken).ConfigureAwait(false);
            }
            bool changed = false;
            //if (existingStreamsInDb.Count != 0)
            //{
            //    List<SMChannel> smChannels = await Repository.SMChannel.GetQuery(true).Where(a => a.M3UFileId == request.M3UFileId && a.StreamID != null && existingStreamsIDsInDb.Contains(a.StreamID)).ToListAsync(cancellationToken: cancellationToken);

            //    foreach (SMChannel smChannel in smChannels)
            //    {
            //        SMStream? stream = existingStreamsInDb.Find(a => a.Id == smChannel.StreamID);
            //        if (stream == null)
            //        {
            //            continue;
            //        }

            //        if (smChannel.Name != stream.Name)
            //        {
            //            changed = true;
            //            smChannel.Name = stream.Name;
            //        }

            //        //if (smChannel.Logo != stream.Logo)
            //        //{
            //        //    changed = true;
            //        //    smChannel.Logo = stream.Logo;
            //        //}

            //        //if (smChannel.VideoOutputProfileName != stream.VideoOutputProfileName)
            //        //{
            //        //    changed = true;
            //        //    smChannel.VideoOutputProfileName = stream.VideoOutputProfileName;
            //        //}

            //        if (smChannel.StationId != stream.StationId)
            //        {
            //            changed = true;
            //            smChannel.StationId = stream.StationId;
            //        }

            //    }
            //    if (changed)
            //    {
            //        await Repository.SaveAsync();
            //    }
            //}

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
