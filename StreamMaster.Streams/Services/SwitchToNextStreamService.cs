using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.Streams.Services;

public sealed class SwitchToNextStreamService(ILogger<SwitchToNextStreamService> logger, IIntroPlayListBuilder introPlayListBuilder, ICustomPlayListBuilder customPlayListBuilder, IOptionsMonitor<Setting> intSettings, IServiceProvider serviceProvider) : ISwitchToNextStreamService
{
    public async Task<bool> SetNextStreamAsync(IStreamStatus ChannelStatus, string? OverrideSMStreamId = null)
    {
        ChannelStatus.FailoverInProgress = true;

        ChannelStatus.OverrideSMStreamId = OverrideSMStreamId;
        Setting _settings = intSettings.CurrentValue;

        if (_settings.ShowIntros > 0)
        {
            if (
                (_settings.ShowIntros == 1 && ChannelStatus.IsFirst) ||
                (_settings.ShowIntros == 2 && !ChannelStatus.PlayedIntro)
                )
            {
                CustomStreamNfo? intro = introPlayListBuilder.GetRandomIntro(ChannelStatus.IsFirst ? null : ChannelStatus.IntroIndex);

                ChannelStatus.IsFirst = false;
                ChannelStatus.PlayedIntro = true;

                if (intro != null)
                {
                    logger.LogDebug("Exiting with : {Id} {Name}", intro.Movie.Id, intro.Movie.Title);
                    string streamId = $"{IntroPlayListBuilder.IntroIDPrefix}{intro.Movie.Title}";
                    SMStreamInfo siOverRide = new()
                    {
                        Id = streamId,
                        Name = intro.Movie.Title,
                        Url = intro.VideoFileName,
                        IsCustomStream = true,
                        ClientUserAgent = intSettings.CurrentValue.SourceClientUserAgent,
                        M3UFileId = EPGHelper.CustomPlayListId
                    };

                    ChannelStatus.SetSMStreamInfo(siOverRide);
                    return true;
                }
            }
        }

        ChannelStatus.PlayedIntro = false;

        if (ChannelStatus.SMChannel.IsCustomStream)
        {
            ChannelStatus.CustomPlayList = customPlayListBuilder.GetCustomPlayList(ChannelStatus.SMChannel.Name);

            if (ChannelStatus.CustomPlayList == null)
            {
                return false;
            }

            //ChannelStatus.SMChannel.CurrentRank++;

            //if (ChannelStatus.SMChannel.CurrentRank >= ChannelStatus.CustomPlayList.CustomStreamNfos.Count)
            //{
            //    ChannelStatus.SMChannel.CurrentRank = 0;
            //}

            (CustomStreamNfo StreamNfo, int SecondsIn) = customPlayListBuilder.GetCurrentVideoAndElapsedSeconds(ChannelStatus.CustomPlayList.Name);

            string clientUserAgent = !string.IsNullOrEmpty(ChannelStatus.ClientUserAgent) ? ChannelStatus.ClientUserAgent : intSettings.CurrentValue.SourceClientUserAgent;
            SMStreamInfo si = new()
            {
                Id = StreamNfo.Movie.Title,
                Name = StreamNfo.Movie.Title,
                Url = StreamNfo.VideoFileName,
                IsCustomStream = true,
                SecondsIn = SecondsIn,
                ClientUserAgent = clientUserAgent,
                M3UFileId = EPGHelper.CustomPlayListId
            };
            //SMStreamInfo si = new()
            //{
            //    Id = ChannelStatus.SMChannel.SourceName,
            //    SourceName = ChannelStatus.SMChannel.SourceName,
            //    Url = ChannelStatus.SMChannel.SourceName,
            //    IsCustomStream = true,
            //    ClientUserAgent = clientUserAgent,
            //    M3UFileId = EPGHelper.CustomPlayListId
            //};

            ChannelStatus.SetSMStreamInfo(si);
            return await Task.FromResult(true);
        }
        else
        {
            if (!string.IsNullOrEmpty(OverrideSMStreamId))
            {
                using IServiceScope scope = serviceProvider.CreateScope();
                IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

                Dictionary<int, M3UFileDto> m3uFilesRepo = (await repository.M3UFile.GetM3UFiles().ConfigureAwait(false))
                                    .ToDictionary(m => m.Id);

                SMStreamDto? smOverRideStream = repository.SMStream.GetSMStream(OverrideSMStreamId);
                if (smOverRideStream == null)
                {
                    return false;
                }

                string clientUserAgentOverRide = !string.IsNullOrEmpty(smOverRideStream.ClientUserAgent) ? smOverRideStream.ClientUserAgent : intSettings.CurrentValue.SourceClientUserAgent;

                logger.LogDebug("Exiting with : {Id} {Name}", smOverRideStream.Id, smOverRideStream.Name);
                SMStreamInfo siOverRide = new()
                {
                    Id = smOverRideStream.Id,
                    Name = smOverRideStream.Name,
                    Url = smOverRideStream.Url,
                    IsCustomStream = smOverRideStream.IsCustomStream,
                    ClientUserAgent = clientUserAgentOverRide,
                    M3UFileId = smOverRideStream.M3UFileId
                };

                ChannelStatus.SetSMStreamInfo(siOverRide);
                return true;
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
                return false;
            }

            List<SMStreamDto> smStreams = [.. channel.SMStreams.OrderBy(a => a.Rank)];

            //for (int i = 0; i < smStreams.Count; i++)
            //{
            channel.CurrentRank = (channel.CurrentRank + 1) % smStreams.Count;
            SMStreamDto smStream = smStreams[channel.CurrentRank];
            logger.LogDebug("Exiting with : {Id} {Name}", smStream.Id, smStream.Name);

            string clientUserAgent = !string.IsNullOrEmpty(smStream.ClientUserAgent) ? smStream.ClientUserAgent : intSettings.CurrentValue.SourceClientUserAgent;

            SMStreamInfo si = new()
            {
                Id = smStream.Id,
                Name = smStream.Name,
                Url = smStream.Url,
                IsCustomStream = smStream.IsCustomStream,
                ClientUserAgent = clientUserAgent,
                M3UFileId = smStream.M3UFileId
            };

            ChannelStatus.SetSMStreamInfo(si);
            return true;
            //}
        }
    }
}
