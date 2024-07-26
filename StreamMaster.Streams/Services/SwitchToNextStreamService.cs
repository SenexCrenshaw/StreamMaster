using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Configuration;
using StreamMaster.PlayList;

namespace StreamMaster.Streams.Services;

public sealed class SwitchToNextStreamService(ILogger<SwitchToNextStreamService> logger, IOptionsMonitor<Setting> intSettings, IServiceProvider serviceProvider) : ISwitchToNextStreamService
{
    public async Task<bool> SetNextStreamAsync(IM3U8ChannelStatus ChannelStatus, string? overrideSMStreamId = null)
    {
        if (ChannelStatus.FailoverInProgress)
        {
            return false;
        }

        ChannelStatus.FailoverInProgress = true;
        if (!string.IsNullOrEmpty(overrideSMStreamId))
        {
            ChannelStatus.OverrideSMStreamId = overrideSMStreamId;
        }

        if (ChannelStatus.SMChannel.IsCustomStream)
        {
            ChannelStatus.ClientUserAgent = intSettings.CurrentValue.ClientUserAgent;
            if (ChannelStatus.CustomPlayList == null)
            {
                return false;
            }

            ChannelStatus.SMChannel.CurrentRank++;

            if (ChannelStatus.SMChannel.CurrentRank >= ChannelStatus.CustomPlayList.CustomStreamNfos.Count)
            {
                ChannelStatus.SMChannel.CurrentRank = 0;
            }

            CustomStreamNfo nfo = ChannelStatus.CustomPlayList.CustomStreamNfos[ChannelStatus.SMChannel.CurrentRank];
            IdNameUrl si = new()
            {
                Id = nfo.Movie.Title,
                Name = nfo.Movie.Title,
                Url = ChannelStatus.CustomPlayList.CustomStreamNfos[ChannelStatus.SMChannel.CurrentRank].VideoFileName,
                IsCustomStream = true
            };

            ChannelStatus.SetSMStreamInfo(si);
            return await Task.FromResult(true);

        }
        else
        {
            Setting _settings = intSettings.CurrentValue;

            using IServiceScope scope = serviceProvider.CreateScope();
            IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

            Dictionary<int, M3UFileDto> m3uFilesRepo = (await repository.M3UFile.GetM3UFiles().ConfigureAwait(false))
                                .ToDictionary(m => m.Id);

            if (!string.IsNullOrEmpty(overrideSMStreamId))
            {
                SMStreamDto? smStream = repository.SMStream.GetSMStream(overrideSMStreamId);
                if (smStream == null)
                {
                    return false;
                }

                //if (!m3uFilesRepo.TryGetValue(smStream.M3UFileId, out M3UFileDto? m3uFile))
                //{
                //    if (GetGlobalStreamsCount() >= _settings.GlobalStreamLimit)
                //    {
                //        logger.LogInformation("Max global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), smStream.Url);
                //        return false;
                //    }

                //    channelStatus.SetIsGlobal();
                //    logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
                //}
                //else if (m3uFile.MaxStreamCount > 0)
                //{
                //    int allStreamsCount = GetCurrentStreamCountForM3UFile(m3uFile.Id);
                //    if (allStreamsCount >= m3uFile.MaxStreamCount)
                //    {
                //        logger.LogInformation("Max stream count {AllStreamsCount}/{MaxStreams} reached for stream: {StreamUrl}", allStreamsCount, m3uFile.MaxStreamCount, smStream.Url);
                //        return false;
                //    }
                //}

                logger.LogDebug("Exiting SetNextChildVideoStream with to Return: {Id} {Name}", smStream.Id, smStream.Name);
                IdNameUrl si = new()
                {
                    Id = smStream.Id,
                    Name = smStream.Name,
                    Url = smStream.Url,
                    IsCustomStream = true
                };

                ChannelStatus.SetSMStreamInfo(si);
                return false;
            }

            SMChannelDto channel = ChannelStatus.SMChannel;
            if (channel.SMStreams.Count == 0)
            {
                logger.LogError("SetNextChildVideoStream could not get videoStreams for id {ParentVideoStreamId}", channel.Id);
                ChannelStatus.SetSMStreamInfo(null);
                return false;
            }

            if (channel.CurrentRank + 1 >= channel.SMStreams.Count)
            {
                logger.LogInformation("SetNextChildVideoStream no more streams for id {ParentVideoStreamId}, exiting", channel.Id);
                ChannelStatus.SetSMStreamInfo(null);
                return false;
            }

            List<SMStreamDto> smStreams = channel.SMStreams.OrderBy(a => a.Rank).ToList();

            for (int i = 0; i < smStreams.Count; i++)
            {
                channel.CurrentRank = (channel.CurrentRank + 1) % smStreams.Count;
                SMStreamDto smStream = smStreams[channel.CurrentRank];

                //if (!m3uFilesRepo.TryGetValue(toReturn.M3UFileId, out M3UFileDto? m3uFile))
                //{
                //    if (GetGlobalStreamsCount() >= _settings.GlobalStreamLimit)
                //    {
                //        logger.LogInformation("Max global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), toReturn.Url);
                //        continue;
                //    }

                //    channelStatus.SetIsGlobal();
                //    logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
                //}
                //else if (m3uFile.MaxStreamCount > 0)
                //{
                //    int allStreamsCount = GetCurrentStreamCountForM3UFile(m3uFile.Id);
                //    if (allStreamsCount >= m3uFile.MaxStreamCount)
                //    {
                //        logger.LogInformation("Max stream count {AllStreamsCount}/{MaxStreams} reached for stream: {StreamUrl}", allStreamsCount, m3uFile.MaxStreamCount, toReturn.Url);
                //        continue;
                //    }
                //}

                logger.LogDebug("Exiting SetNextChildVideoStream with to Return: {Id} {Name}", smStream.Id, smStream.Name);
                //SMStreamDto a = mapper.Map<SMStreamDto>(toReturn);
                //channelStatus.SetCurrentSMStream(a);
                IdNameUrl si = new()
                {
                    Id = smStream.Id,
                    Name = smStream.Name,
                    Url = smStream.Url,
                    IsCustomStream = true
                };

                ChannelStatus.SetSMStreamInfo(si);
                return true;
            }
        }
        return false;
    }
}
