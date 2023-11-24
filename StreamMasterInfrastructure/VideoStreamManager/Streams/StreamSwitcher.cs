using AutoMapper;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Extensions;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructure.VideoStreamManager.Streams;

public class StreamSwitcher(ILogger<StreamSwitcher> logger, IChannelService channelService, IServiceProvider serviceProvider, IMemoryCache memoryCache, IStreamManager streamManager) : IStreamSwitcher
{
    public async Task<bool> SwitchToNextVideoStreamAsync(string ChannelVideoStreamId, string? overrideNextVideoStreamId = null)
    {
        logger.LogDebug("Start SwitchToNextVideoStream {ChannelVideoStreamId}", ChannelVideoStreamId);
        IChannelStatus? channelStatus = channelService.GetChannelStatus(ChannelVideoStreamId);
        if (channelStatus is null)
        {
            logger.LogError("SwitchToNextVideoStream could not get channelStatus for id {ChannelVideoStreamId}", ChannelVideoStreamId);
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to channelStatus being null");
            return false;
        }
        logger.LogDebug("Starting SwitchToNextVideoStream with channelStatus: {channelStatus} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelStatus, overrideNextVideoStreamId);

        channelStatus.FailoverInProgress = true;

        if (!await TokenExtensions.ApplyDelay())
        {
            logger.LogInformation("Task was cancelled");
            channelStatus.FailoverInProgress = false;
            return false;
        }
        IStreamHandler? oldStreamHandler = streamManager.GetStreamHandler(channelStatus.CurrentVideoStreamId);

        VideoStreamDto? videoStreamDto = await RetrieveNextChildVideoStream(channelStatus, overrideNextVideoStreamId);
        if (videoStreamDto is null)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to videoStreamDto being null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        IStreamHandler? newStreamHandler = await streamManager.GetOrCreateStreamHandler(videoStreamDto, channelStatus.Rank);

        if (newStreamHandler is null)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to channelStatus. newStreamHandler is null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        if (oldStreamHandler is not null)
        {
            streamManager.MoveClientStreamers(oldStreamHandler.GetClientStreamerClientIds(), newStreamHandler);
        }
        channelStatus.CurrentVideoStreamName = videoStreamDto.User_Tvg_name;
        channelStatus.CurrentVideoStreamId = videoStreamDto.Id;
        channelStatus.FailoverInProgress = false;


        logger.LogDebug("Finished SwitchToNextVideoStream");

        return true;
    }

    private int GetGlobalStreamsCount()
    {
        return channelService.GetGlobalStreamsCount();
    }

    private async Task<VideoStreamDto?> HandleOverrideStream(string overrideNextVideoStreamId, IRepositoryWrapper repository, IChannelStatus channelStatus, IMapper mapper)
    {
        Setting setting = memoryCache.GetSetting();

        VideoStreamDto? vs = await repository.VideoStream.GetVideoStreamById(overrideNextVideoStreamId);
        if (vs == null)
        {
            logger.LogError("HandleOverrideStream could not get videoStream for id {VideoStreamId}", overrideNextVideoStreamId);
            logger.LogDebug("Exiting HandleOverrideStream with null due to newVideoStream being null");
            return null;
        }

        //if (newVideoStream == null)
        //{
        //    logger.LogError("HandleOverrideStream could not get videoStream for id {VideoStreamId}", overrideNextVideoStreamId);
        //    logger.LogDebug("Exiting HandleOverrideStream with null due to newVideoStream being null");
        //    return null;
        //}

        M3UFile? m3uFile = await repository.M3UFile.GetM3UFileById(vs.M3UFileId).ConfigureAwait(false);

        //M3UFileDto? m3uFile = m3uFilesRepo.Find(a => a.Id == vs.M3UFileId);
        if (m3uFile == null)
        {
            if (GetGlobalStreamsCount() >= setting.GlobalStreamLimit)
            {
                logger.LogInformation("Max Global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), vs.User_Url);
                return null;
            }

            channelStatus.SetIsGlobal();
            logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
            return vs;
        }
        else
        {
            int allStreamsCount = streamManager.GetStreamsCountForM3UFile(vs.M3UFileId);

            if (vs.Id != channelStatus.CurrentVideoStreamId && allStreamsCount >= m3uFile.MaxStreamCount)
            {
                logger.LogInformation("Max stream count {MaxStreams} reached for stream: {StreamUrl}", vs.MaxStreams, vs.User_Url);
            }
            else
            {
                logger.LogDebug("Exiting HandleOverrideStream with newVideoStream: {newVideoStream}", vs);
                return vs;
            }
        }
        return null;
    }

    private async Task<ChildVideoStreamDto?> FetchNextChildVideoStream(IChannelStatus channelStatus, IRepositoryWrapper repository)
    {
        Setting setting = memoryCache.GetSetting();
        (VideoStreamHandlers videoStreamHandler, List<ChildVideoStreamDto> childVideoStreamDtos)? result = await repository.VideoStream.GetStreamsFromVideoStreamById(channelStatus.ChannelVideoStreamId);
        if (result == null)
        {
            logger.LogError("FetchNextChildVideoStream could not get videoStreams for id {ParentVideoStreamId}", channelStatus.ChannelVideoStreamId);
            logger.LogDebug("Exiting FetchNextChildVideoStream with null due to result being null");
            return null;
        }

        ChildVideoStreamDto[] videoStreams = result.Value.childVideoStreamDtos.OrderBy(a => a.Rank).ToArray();
        if (!videoStreams.Any())
        {
            return null;
        }

        VideoStreamHandlers videoHandler = result.Value.videoStreamHandler == VideoStreamHandlers.SystemDefault ? VideoStreamHandlers.Loop : result.Value.videoStreamHandler;

        if (channelStatus.Rank >= videoStreams.Length)
        {
            channelStatus.Rank = 0;
        }

        while (channelStatus.Rank < videoStreams.Length)
        {
            ChildVideoStreamDto toReturn = videoStreams[channelStatus.Rank++];
            List<M3UFileDto> m3uFilesRepo = await repository.M3UFile.GetM3UFiles().ConfigureAwait(false);

            M3UFileDto? m3uFile = m3uFilesRepo.Find(a => a.Id == toReturn.M3UFileId);
            if (m3uFile == null)
            {
                if (GetGlobalStreamsCount() >= setting.GlobalStreamLimit)
                {
                    logger.LogInformation("Max Global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), toReturn.User_Url);
                    continue;
                }

                channelStatus.SetIsGlobal();
                logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
            }
            else
            {
                int allStreamsCount = streamManager.GetStreamsCountForM3UFile(toReturn.M3UFileId);
                if (allStreamsCount >= m3uFile.MaxStreamCount)
                {
                    logger.LogInformation("Max stream count {MaxStreams} reached for stream: {StreamUrl}", toReturn.MaxStreams, toReturn.User_Url);
                    continue;
                }
            }
            logger.LogDebug("Exiting FetchNextChildVideoStream with to Return: {Id} {Name}", toReturn.Id, toReturn.User_Tvg_name);
            channelStatus.CurrentVideoStreamId = toReturn.Id;
            channelStatus.CurrentVideoStreamName = toReturn.User_Tvg_name;
            return toReturn;
        }

        logger.LogDebug("Exiting FetchNextChildVideoStream with null due to no suitable videoStream found");
        return null;
    }

    private async Task<VideoStreamDto?> RetrieveNextChildVideoStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {
        logger.LogDebug("Starting RetrieveNextChildVideoStream with channelStatus: {VideoStreamName} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelStatus.CurrentVideoStreamName, overrideNextVideoStreamId);

        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        IMapper mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        if (!string.IsNullOrEmpty(overrideNextVideoStreamId))
        {
            VideoStreamDto? handled = await HandleOverrideStream(overrideNextVideoStreamId, repository, channelStatus, mapper);
            if (handled != null || (handled == null && !string.IsNullOrEmpty(overrideNextVideoStreamId)))
            {
                return handled;
            }
        }
        VideoStreamDto? result = await FetchNextChildVideoStream(channelStatus, repository);
        if (result != null)
        {
            return result;
        }
        logger.LogDebug("Exiting RetrieveNextChildVideoStream with null due to no suitable videoStream found");
        return null;
    }
}