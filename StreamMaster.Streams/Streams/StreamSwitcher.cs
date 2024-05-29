using AutoMapper;

using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Streams;

public sealed class StreamSwitcher(ILogger<StreamSwitcher> logger, IClientStreamerManager clientStreamerManager, IChannelService channelService, IServiceProvider serviceProvider, IOptionsMonitor<Setting> intsettings, IStreamManager streamManager)
    : IStreamSwitcher
{
    private readonly Setting settings = intsettings.CurrentValue;


    public async Task<bool> SwitchToNextVideoStreamAsync(SMChannel smChannel, string? overrideNextVideoStreamId = null)
    {

        IChannelStatus? channelStatus = channelService.GetChannelStatus(smChannel.Id);
        if (channelStatus is null)
        {
            logger.LogError("SwitchToNextVideoStream could not get channel for id {ChannelVideoStreamId}", smChannel.Id);
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to channelStatus being null");
            return false;
        }

        if (channelStatus.FailoverInProgress)
        {
            //logger.LogDebug("Exiting SwitchToNextVideoStream with false due to FailoverInProgress being true");
            return false;
        }

        channelStatus.FailoverInProgress = true;
        if (!string.IsNullOrEmpty(overrideNextVideoStreamId))
        {
            channelStatus.OverrideVideoStreamId = overrideNextVideoStreamId;
        }

        logger.LogDebug("Starting SwitchToNextVideoStream with channelStatus: {channelStatus} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelStatus, overrideNextVideoStreamId);


        IStreamHandler? oldStreamHandler = streamManager.GetStreamHandler(channelStatus.CurrentSMStream?.Url);


        SMStream? smStream = await RetrieveNextChildVideoStream(channelStatus, overrideNextVideoStreamId);
        if (smStream is null)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to smStream being null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        if (oldStreamHandler != null && oldStreamHandler.SMStream.Id == smStream.Id)
        {
            logger.LogDebug("Matching ids, stopping original stream");
            oldStreamHandler.SetFailed();

            channelStatus.FailoverInProgress = false;
            oldStreamHandler.Stop();
            return true;
        }

        IStreamHandler? newStreamHandler = await streamManager.GetOrCreateStreamHandler(smChannel, smStream, channelStatus.Rank);

        if (newStreamHandler is null)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to channelStatus. newStreamHandler is null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        if (channelStatus.CurrentSMStream is not null && oldStreamHandler is not null)
        {
            await streamManager.MoveClientStreamers(oldStreamHandler, newStreamHandler);
        }
        else
        {
            await clientStreamerManager.AddClientsToHandler(smChannel.Id, newStreamHandler);
        }

        //channelStatus.SetCurrentSMStream(smStream);
        channelStatus.FailoverInProgress = false;

        logger.LogDebug("Finished SwitchToNextVideoStream");

        return true;
    }

    private int GetGlobalStreamsCount()
    {
        return channelService.GetGlobalStreamsCount();
    }

    //private async Task<SMStream?> HandleOverrideStream(string overrideNextVideoStreamId, IRepositoryWrapper repository, IChannelStatus channelStatus)
    //{

    //    SMStream? vs = repository.SMStream.GetSMStream(overrideNextVideoStreamId);
    //    if (vs == null)
    //    {
    //        logger.LogError("HandleOverrideStream could not get videoStream for id {VideoStreamId}", overrideNextVideoStreamId);
    //        logger.LogDebug("Exiting HandleOverrideStream with null due to newVideoStream being null");
    //        return null;
    //    }

    //    M3UFile? m3uFile = await repository.M3UFile.GetM3UFile(vs.M3UFileId).ConfigureAwait(false);

    //    if (m3uFile == null)
    //    {
    //        if (GetGlobalStreamsCount() > settings.GlobalStreamLimit)
    //        {
    //            logger.LogInformation("Max Global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), vs.Url);
    //            return null;
    //        }

    //        channelStatus.SetIsGlobal();
    //        logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
    //        return vs;
    //    }

    //    int allStreamsCount = streamManager.GetStreamsCountForM3UFile(vs.M3UFileId);

    //    if (vs.Id != channelStatus.CurrentSMStream.Id && allStreamsCount >= m3uFile.MaxStreamCount)
    //    {
    //        logger.LogInformation("Max stream count {MaxStreams} reached for stream: {StreamUrl}", 999, vs.Url);
    //    }
    //    else
    //    {
    //        logger.LogDebug("Exiting HandleOverrideStream with newVideoStream: {newVideoStream}", vs);
    //        return vs;
    //    }

    //    return null;
    //}

    //private async Task<SMStream?> FetchNextChildVideoStream(IChannelStatus channelStatus, IRepositoryWrapper repository)
    //{
    //    var smStreams = channelStatus.CurrentSMStream;
    //    (VideoStreamHandlers videoStreamHandler, List<SMStream> smStreams)? result = await repository.SMStream.GetStreamsFromVideoStreamById(channelStatus.ChannelSMStreamId);
    //    if (result == null)
    //    {
    //        logger.LogError("FetchNextChildVideoStream could not get videoStreams for id {ParentVideoStreamId}", channelStatus.ChannelSMStreamId);
    //        logger.LogDebug("Exiting FetchNextChildVideoStream with null due to result being null");
    //        return null;
    //    }

    //    VideoStreamDto[] videoStreams = [.. result.Value.childVideoStreamDtos.OrderBy(a => a.Rank)];
    //    if (!videoStreams.Any())
    //    {
    //        return null;
    //    }

    //    VideoStreamHandlers videoHandler = result.Value.videoStreamHandler == VideoStreamHandlers.SystemDefault ? VideoStreamHandlers.Loop : result.Value.videoStreamHandler;

    //    if (channelStatus.Rank >= videoStreams.Length)
    //    {
    //        channelStatus.Rank = 0;
    //    }

    //    while (channelStatus.Rank < videoStreams.Length)
    //    {
    //        VideoStreamDto toReturn = videoStreams[channelStatus.Rank++];
    //        List<M3UFileDto> m3uFilesRepo = await repository.M3UFile.GetM3UFiles().ConfigureAwait(false);

    //        M3UFileDto? m3uFile = m3uFilesRepo.Find(a => a.Id == toReturn.M3UFileId);
    //        if (m3uFile == null)
    //        {
    //            if (GetGlobalStreamsCount() >= settings.GlobalStreamLimit)
    //            {
    //                logger.LogInformation("Max Global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), toReturn.User_Url);
    //                continue;
    //            }

    //            channelStatus.SetIsGlobal();
    //            logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
    //        }
    //        else
    //        {
    //            int allStreamsCount = streamManager.GetStreamsCountForM3UFile(toReturn.M3UFileId);
    //            if (allStreamsCount >= m3uFile.MaxStreamCount)
    //            {
    //                logger.LogInformation("Max stream count {MaxStreams} reached for stream: {StreamUrl}", toReturn.MaxStreams, toReturn.User_Url);
    //                continue;
    //            }
    //        }
    //        logger.LogDebug("Exiting FetchNextChildVideoStream with to Return: {Id} {Name}", toReturn.Id, toReturn.User_Tvg_name);

    //        return toReturn;
    //    }

    //    logger.LogDebug("Exiting FetchNextChildVideoStream with null due to no suitable videoStream found");
    //    return null;
    //}

    private async Task<SMStream?> RetrieveNextChildVideoStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {

        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        IMapper mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        await channelService.SetNextChildVideoStream(channelStatus.Id, overrideNextVideoStreamId);
        logger.LogDebug("Starting RetrieveNextChildVideoStream with channelStatus: {VideoStreamName} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelStatus.CurrentSMStream.Name, overrideNextVideoStreamId);

        return channelStatus.CurrentSMStream;

        //if (!string.IsNullOrEmpty(overrideNextVideoStreamId))
        //{
        //    VideoStreamDto? handled = await HandleOverrideStream(overrideNextVideoStreamId, repository, channelStatus);
        //    if (handled != null)
        //    {
        //        return handled;
        //    }
        //    logger.LogDebug("Exiting RetrieveNextChildVideoStream with null due to no overrideNextVideoStreamId not found");
        //    return null;
        //}
        //VideoStreamDto? result = await FetchNextChildVideoStream(channelStatus, repository);
        //if (result != null)
        //{
        //    return result;
        //}
        //logger.LogDebug("Exiting RetrieveNextChildVideoStream with null due to no suitable videoStream found");
        //return null;
    }
}