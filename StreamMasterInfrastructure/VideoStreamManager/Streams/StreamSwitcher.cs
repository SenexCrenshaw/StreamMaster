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

public class StreamSwitcher(ILogger<StreamSwitcher> logger, IClientStreamerManager clientStreamerManager, IServiceProvider serviceProvider, IMemoryCache memoryCache, IStreamManager streamManager) : IStreamSwitcher
{
    private async Task<bool> UpdateStreamHandler(IChannelStatus channelStatus, ChildVideoStreamDto childVideoStreamDto)
    {
        try
        {
            IStreamHandler? handler = await streamManager.GetOrCreateStreamHandler(childVideoStreamDto, channelStatus.Rank);

            return handler != null;
        }
        catch (TaskCanceledException)
        {
            logger.LogInformation("Task was cancelled");
            throw;
        }
    }

    public async Task<bool> SwitchToNextVideoStreamAsync(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {
        logger.LogDebug("Starting SwitchToNextVideoStream with channelStatus: {channelStatus} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelStatus, overrideNextVideoStreamId);

        channelStatus.FailoverInProgress = true;

        if (!await TokenExtensions.ApplyDelay())
        {
            logger.LogInformation("Task was cancelled");
            channelStatus.FailoverInProgress = false;
            return false;
        }
        IStreamHandler? oldStreamHandler = streamManager.GetStreamHandler(channelStatus.VideoStreamId);

        ChildVideoStreamDto? childVideoStreamDto = await RetrieveNextChildVideoStream(channelStatus, overrideNextVideoStreamId);
        if (childVideoStreamDto is null)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to childVideoStreamDto being null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        if (!await UpdateStreamHandler(channelStatus, childVideoStreamDto))
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to channelStatus.StreamInformation being null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        IStreamHandler? newStreamHandler = streamManager.GetStreamHandler(childVideoStreamDto.Id);

        if (oldStreamHandler is not null && newStreamHandler is not null)
        {
            clientStreamerManager.MoveClientStreamers(oldStreamHandler, newStreamHandler);
        }

        channelStatus.FailoverInProgress = false;
        logger.LogDebug("Finished SwitchToNextVideoStream");

        return true;
    }

    private int GetGlobalStreamsCount()
    {
        return 0; // channelService.GetGlobalStreamsCount();
    }

    private async Task<ChildVideoStreamDto?> HandleOverrideStream(string overrideNextVideoStreamId, IRepositoryWrapper repository, IChannelStatus channelStatus, IMapper mapper)
    {
        Setting setting = memoryCache.GetSetting();

        VideoStreamDto? vs = await repository.VideoStream.GetVideoStreamById(overrideNextVideoStreamId);
        if (vs == null)
        {
            logger.LogError("HandleOverrideStream could not get videoStream for id {VideoStreamId}", overrideNextVideoStreamId);
            logger.LogDebug("Exiting HandleOverrideStream with null due to newVideoStream being null");
            return null;
        }

        ChildVideoStreamDto newVideoStream = mapper.Map<ChildVideoStreamDto>(vs);
        if (newVideoStream == null)
        {
            logger.LogError("HandleOverrideStream could not get videoStream for id {VideoStreamId}", overrideNextVideoStreamId);
            logger.LogDebug("Exiting HandleOverrideStream with null due to newVideoStream being null");
            return null;
        }

        List<M3UFileDto> m3uFilesRepo = await repository.M3UFile.GetM3UFiles();

        M3UFileDto? m3uFile = m3uFilesRepo.Find(a => a.Id == newVideoStream.M3UFileId);
        if (m3uFile == null)
        {
            if (GetGlobalStreamsCount() >= setting.GlobalStreamLimit)
            {
                logger.LogInformation("Max Global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), newVideoStream.User_Url);
                return null;
            }

            channelStatus.SetIsGlobal();
            logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
            return newVideoStream;
        }
        else
        {
            int allStreamsCount = streamManager.GetStreamsCountForM3UFile(newVideoStream.M3UFileId);

            if (newVideoStream.Id != channelStatus.VideoStreamId && allStreamsCount >= m3uFile.MaxStreamCount)
            {
                logger.LogInformation("Max stream count {MaxStreams} reached for stream: {StreamUrl}", newVideoStream.MaxStreams, newVideoStream.User_Url);
            }
            else
            {
                logger.LogDebug("Exiting HandleOverrideStream with newVideoStream: {newVideoStream}", newVideoStream);
                return newVideoStream;
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
            //logger.LogError("FetchNextChildVideoStream could not get child videoStreams for id {ChannelVideoStreamId}", channelStatus.ChannelVideoStreamId);
            //logger.LogDebug("Exiting FetchNextChildVideoStream with null due to no child videoStreams found");
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
            logger.LogDebug("Exiting FetchNextChildVideoStream with toReturn: {toReturn}", toReturn);
            channelStatus.VideoStreamId = toReturn.Id;
            channelStatus.VideoStreamName = toReturn.User_Tvg_name;
            return toReturn;
        }

        logger.LogDebug("Exiting FetchNextChildVideoStream with null due to no suitable videoStream found");
        return null;
    }

    private async Task<ChildVideoStreamDto?> RetrieveNextChildVideoStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {
        logger.LogDebug("Starting RetrieveNextChildVideoStream with channelStatus: {VideoStreamName} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelStatus.VideoStreamName, overrideNextVideoStreamId);

        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        IMapper mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        if (!string.IsNullOrEmpty(overrideNextVideoStreamId))
        {
            ChildVideoStreamDto? handled = await HandleOverrideStream(overrideNextVideoStreamId, repository, channelStatus, mapper);
            if (handled != null || (handled == null && !string.IsNullOrEmpty(overrideNextVideoStreamId)))
            {
                return handled;
            }
        }
        ChildVideoStreamDto? result = await FetchNextChildVideoStream(channelStatus, repository);
        if (result != null)
        {
            return result;
        }
        logger.LogDebug("Exiting RetrieveNextChildVideoStream with null due to no suitable videoStream found");
        return null;
    }
}