using FluentValidation;

using Microsoft.AspNetCore.Http;

using System.Collections.Concurrent;

namespace StreamMaster.Application.StreamGroups.Queries;


[RequireAll]
public record GetStreamGroupVideoConfigs(int StreamGroupId, int StreamGroupProfileId) : IRequest<(List<VideoStreamConfig> videoStreamConfigs, OutputProfile outputProfile)>;
public class GetStreamGroupVideoConfigsHandler(IHttpContextAccessor httpContextAccessor, ISender sender, IRepositoryWrapper repositoryWrapper, IEPGHelper epgHelper, IXMLTVBuilder xMLTVBuilder, ILogger<GetStreamGroupVideoConfigs> logger, ISchedulesDirectDataService schedulesDirectDataService, IRepositoryWrapper Repository, IOptionsMonitor<Setting> intsettings)
    : IRequestHandler<GetStreamGroupVideoConfigs, (List<VideoStreamConfig> videoStreamConfigs, OutputProfile outputProfile)>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    //private readonly ParallelOptions parallelOptions = new()
    //{
    //    MaxDegreeOfParallelism = Environment.ProcessorCount
    //};
    private readonly ConcurrentDictionary<int, VideoStreamConfig> existingNumbers = new();
    private readonly ConcurrentHashSet<int> usedNumbers = [];
    private int currentChannelNumber = 0;

    private int GetNextChannelNumber(int channelNumber, bool ignoreExisting)
    {
        if (ignoreExisting)
        {
            return getNext();
        }

        if (existingNumbers.ContainsKey(channelNumber))
        {
            if (usedNumbers.Add(channelNumber))
            {
                return channelNumber;
            }
        }

        return getNext();
    }

    private int getNext()
    {
        ++currentChannelNumber;
        while (!usedNumbers.Add(currentChannelNumber))
        {
            ++currentChannelNumber;
        }
        return currentChannelNumber;
    }

    [LogExecutionTimeAspect]
    public async Task<(List<VideoStreamConfig> videoStreamConfigs, OutputProfile outputProfile)> Handle(GetStreamGroupVideoConfigs request, CancellationToken cancellationToken)
    {

        StreamGroup? streamGroup = Repository.StreamGroup.GetStreamGroup(request.StreamGroupId);

        if (streamGroup == null)
        {
            return new();
        }

        List<SMChannel> smChannels = (request.StreamGroupId < 2 ?
             await Repository.SMChannel.GetQuery().ToListAsync(cancellationToken: cancellationToken) :
             await Repository.SMChannel.GetSMChannelsFromStreamGroup(request.StreamGroupId)).Where(a => !a.IsHidden).ToList();

        if (smChannels.Count == 0)
        {
            return new();
        }

        currentChannelNumber = 0;

        StreamGroupProfile sgProfile = Repository.StreamGroupProfile.GetStreamGroupProfile(request.StreamGroupId, request.StreamGroupProfileId) ?? new StreamGroupProfile();
        DataResponse<OutputProfileDto> profileRequest = await sender.Send(new GetOutputProfileRequest(sgProfile.OutputProfileName), cancellationToken);
        OutputProfile profile = profileRequest == null ? SettingFiles.DefaultOutputProfileSetting.OutProfiles["Default"] : profileRequest.Data;
        List<VideoStreamConfig> videoStreamConfigs = [];

        logger.LogInformation("GetStreamGroupVideoConfigsHandler: Handling {Count} channels", smChannels.Count);
        //var test = smChannels.SelectMany(a => a.SMStreams.Where(a => a.Rank == 0)).Select(a => new { a.SMChannel, a.SMStream }).ToList();

        foreach (SMChannel? smChannel in smChannels.Where(a => !a.IsHidden))
        {
            SMStream? stream = smChannel.SMStreams.FirstOrDefault(a => a.Rank == 0)?.SMStream;

            videoStreamConfigs.Add(new VideoStreamConfig
            {
                Id = smChannel.Id,
                Name = smChannel.Name,
                EPGId = smChannel.EPGId,
                Logo = smChannel.Logo,
                ChannelNumber = smChannel.ChannelNumber,
                TimeShift = smChannel.TimeShift,
                IsDuplicate = false,
                IsDummy = false,
                M3UFileId = stream?.M3UFileId ?? 0,
                FilePosition = stream?.FilePosition ?? 0
            });
        }

        //if (streamGroup.AutoSetChannelNumbers)
        //{
        //    currentChannelNumber = streamGroup.StartingChannelNumber - 1;
        //    if (!streamGroup.IgnoreExistingChannelNumbers)
        //    {
        //        foreach (var channelNumber in videoStreamConfigs.Where(a => a.ChannelNumber != 0).Select(a => a.ChannelNumber).Distinct())
        //        {
        //            var videoStreamConfig = videoStreamConfigs.First(a => a.ChannelNumber == channelNumber);
        //            existingNumbers.TryAdd(channelNumber, videoStreamConfig);
        //        }
        //    }

        //    foreach (var videoStreamConfig in videoStreamConfigs.OrderBy(a => a.M3UFileId).ThenBy(a => a.FilePosition))
        //    {
        //        var channelNumber = GetNextChannelNumber(videoStreamConfig.ChannelNumber, streamGroup.IgnoreExistingChannelNumbers);
        //        videoStreamConfig.ChannelNumber = channelNumber;
        //    }
        //}
        return (videoStreamConfigs, profile);

    }


}