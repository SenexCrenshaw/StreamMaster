using Microsoft.Extensions.DependencyInjection;
namespace StreamMaster.Streams.Services;


public sealed class SwitchToNextStreamService(ILogger<SwitchToNextStreamService> logger, IStreamGroupService streamGroupService, IOptionsMonitor<CommandProfiles> optionsOutputProfiles, IIntroPlayListBuilder introPlayListBuilder, ICustomPlayListBuilder customPlayListBuilder, IOptionsMonitor<Setting> intSettings, IServiceProvider serviceProvider) : ISwitchToNextStreamService
{
    private static string GetClientUserAgenet(SMChannelDto smChannel, Setting setting)
    {
        string clientUserAgent =
        //!string.IsNullOrEmpty(smStream.ClientUserAgent) ? smStream.ClientUserAgent :
        !string.IsNullOrEmpty(smChannel.ClientUserAgent) ? smChannel.ClientUserAgent :
           setting.ClientUserAgent;

        return clientUserAgent;
    }
    public async Task<bool> SetNextStreamAsync(IStreamStatus ChannelStatus, string? OverrideSMStreamId = null)
    {
        ChannelStatus.FailoverInProgress = true;

        //ChannelStatus.OverrideSMStreamId = OverrideSMStreamId;
        Setting _settings = intSettings.CurrentValue;


        //if ( _settings.ShowIntros != "None" && _settings.ShowIntros != "None" && _settings.ShowIntros != "None")

        if (_settings.ShowIntros != "None")
        {
            if (
                (_settings.ShowIntros == "Once" && ChannelStatus.IsFirst) ||
                (_settings.ShowIntros == "Always" && !ChannelStatus.PlayedIntro)
                )
            {
                CustomStreamNfo? intro = introPlayListBuilder.GetRandomIntro(ChannelStatus.IsFirst ? null : ChannelStatus.IntroIndex);
                CommandProfileDto commandProfileDto = optionsOutputProfiles.CurrentValue.GetProfileDto("SMFFMPEG");
                ChannelStatus.IsFirst = false;
                ChannelStatus.PlayedIntro = true;

                if (intro != null)
                {
                    logger.LogDebug("SetNextStream returning with : {Id} {Name}", intro.Movie.Id, intro.Movie.Title);
                    string streamId = $"{IntroPlayListBuilder.IntroIDPrefix}{intro.Movie.Title}";
                    SMStreamInfo siOverRide = new()
                    {
                        Id = streamId,
                        Name = intro.Movie.Title,
                        Url = intro.VideoFileName,
                        IsCustomStream = true,
                        ClientUserAgent = intSettings.CurrentValue.ClientUserAgent,
                        M3UFileId = EPGHelper.IntroPlayListId,
                        CommandProfile = commandProfileDto
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

            (CustomStreamNfo StreamNfo, int SecondsIn) = customPlayListBuilder.GetCurrentVideoAndElapsedSeconds(ChannelStatus.CustomPlayList.Name);
            string clientUserAgent = GetClientUserAgenet(ChannelStatus.SMChannel, intSettings.CurrentValue);
            CommandProfileDto commandProfileDto = await streamGroupService.GetProfileFromSMChannelDtoAsync(ChannelStatus.StreamGroupId, ChannelStatus.StreamGroupProfileId, ChannelStatus.SMChannel.CommandProfileName);


            SMStreamInfo si = new()
            {
                Id = StreamNfo.Movie.Title,
                Name = StreamNfo.Movie.Title,
                Url = StreamNfo.VideoFileName,
                IsCustomStream = true,
                SecondsIn = SecondsIn,
                ClientUserAgent = clientUserAgent,
                M3UFileId = EPGHelper.CustomPlayListId,
                CommandProfile = commandProfileDto
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

            logger.LogDebug("SetNextStream returning with : {Id} {Name}", si.Id, si.Name);
            ChannelStatus.SetSMStreamInfo(si);
            return await Task.FromResult(true);
        }
        else
        {
            SMChannelDto smChannel = ChannelStatus.SMChannel;
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

                 string clientUserAgentOverRide = GetClientUserAgenet(ChannelStatus.SMChannel, intSettings.CurrentValue);
                CommandProfileDto commandProfileDtoOverRide = await streamGroupService.GetProfileFromSMChannelDtoAsync(ChannelStatus.StreamGroupId, ChannelStatus.StreamGroupProfileId, ChannelStatus.SMChannel.CommandProfileName);


                //string clientUserAgentOverRide = GetClientUserAgenet(smChannel, smOverRideStream, intSettings.CurrentValue);


                logger.LogDebug("Set Next with : {Id} {Name}", smOverRideStream.Id, smOverRideStream.Name);
                SMStreamInfo siOverRide = new()
                {
                    Id = smOverRideStream.Id,
                    Name = smOverRideStream.Name,
                    Url = smOverRideStream.Url,
                    IsCustomStream = smOverRideStream.IsCustomStream,
                    ClientUserAgent = clientUserAgentOverRide,
                    M3UFileId = smOverRideStream.M3UFileId,
                    CommandProfile = commandProfileDtoOverRide
                };

                ChannelStatus.SetSMStreamInfo(siOverRide);
                return true;
            }


            if (smChannel.SMStreams.Count == 0)
            {
                logger.LogError("SetNextChildVideoStream could not get videoStreams for id {ParentVideoStreamId}", smChannel.Id);
                ChannelStatus.SetSMStreamInfo(null);
                return false;
            }

            if (smChannel.CurrentRank + 1 >= smChannel.SMStreams.Count)
            {
                logger.LogInformation("SetNextChildVideoStream no more streams for id {ParentVideoStreamId}, exiting", smChannel.Id);
                return false;
            }

            List<SMStreamDto> smStreams = [.. smChannel.SMStreams.OrderBy(a => a.Rank)];

            //for (int i = 0; i < smStreams.Count; i++)
            //{
            smChannel.CurrentRank = (smChannel.CurrentRank + 1) % smStreams.Count;
            SMStreamDto smStream = smStreams[smChannel.CurrentRank];
            logger.LogDebug("Exiting with : {Id} {Name}", smStream.Id, smStream.Name);

            string clientUserAgent = GetClientUserAgenet(ChannelStatus.SMChannel, intSettings.CurrentValue);
            CommandProfileDto commandProfileDto = await streamGroupService.GetProfileFromSMChannelDtoAsync(ChannelStatus.StreamGroupId, ChannelStatus.StreamGroupProfileId, ChannelStatus.SMChannel.CommandProfileName);

            SMStreamInfo si = new()
            {
                Id = smStream.Id,
                Name = smStream.Name,
                Url = smStream.Url,
                IsCustomStream = smStream.IsCustomStream,
                ClientUserAgent = clientUserAgent,
                M3UFileId = smStream.M3UFileId,
                CommandProfile = commandProfileDto
            };

            ChannelStatus.SetSMStreamInfo(si);
            return true;
            //}
        }
    }
}
