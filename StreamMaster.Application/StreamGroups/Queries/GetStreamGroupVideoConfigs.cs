using FluentValidation;

namespace StreamMaster.Application.StreamGroups.Queries;


[RequireAll]
public record GetStreamGroupVideoConfigs(int StreamGroupId, int StreamGroupProfileId) : IRequest<(List<VideoStreamConfig> videoStreamConfigs, OutputProfile outputProfile)>;
public class GetStreamGroupVideoConfigsHandler(ISender sender, ILogger<GetStreamGroupVideoConfigs> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetStreamGroupVideoConfigs, (List<VideoStreamConfig> videoStreamConfigs, OutputProfile outputProfile)>
{
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

        StreamGroupProfile sgProfile = Repository.StreamGroupProfile.GetStreamGroupProfile(request.StreamGroupId, request.StreamGroupProfileId) ?? new StreamGroupProfile();
        DataResponse<OutputProfileDto> profileRequest = await sender.Send(new GetOutputProfileRequest(sgProfile.OutputProfileName), cancellationToken);
        OutputProfile profile = profileRequest == null ? SettingFiles.DefaultOutputProfileSetting.OutProfiles["Default"] : profileRequest.Data;
        List<VideoStreamConfig> videoStreamConfigs = [];

        logger.LogInformation("GetStreamGroupVideoConfigsHandler: Handling {Count} channels", smChannels.Count);

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

        return (videoStreamConfigs, profile);

    }


}