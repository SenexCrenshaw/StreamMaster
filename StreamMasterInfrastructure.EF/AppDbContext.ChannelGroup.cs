using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.ChannelGroups;
using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.StreamGroups.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructure.EF;

public partial class AppDbContext : IChannelGroupDB
{
    public DbSet<ChannelGroup> ChannelGroups { get; set; }

    public async Task<(ChannelGroupDto? channelGroup, List<VideoStreamDto>? distinctList, List<StreamGroupDto>? streamGroupIds)> UpdateChannelGroup(UpdateChannelGroupRequest request, string url, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await ChannelGroups.FirstOrDefaultAsync(a => a.Name.ToLower() == request.GroupName.ToLower(), cancellationToken: cancellationToken).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return (null, null, null);
        }

        List<VideoStreamDto> beforeResults = VideoStreams
         .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
        .AsNoTracking()
         .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider).ToList();

        var beforeRegexStreams = await GetVideoStreamsByNamePatternAsync(channelGroup.RegexMatch, cancellationToken).ConfigureAwait(false);
        if (beforeRegexStreams != null)
        {
            var mapped = _mapper.Map<List<VideoStreamDto>>(beforeRegexStreams);
            beforeResults.AddRange(mapped);
        }

        if (request.Rank != null)
        {
            channelGroup.Rank = (int)request.Rank;
        }

        bool isChanged = false;

        if (request.IsHidden != null)
        {
            channelGroup.IsHidden = (bool)request.IsHidden;
            await VideoStreams
            .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsHidden, (bool)request.IsHidden), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            isChanged = true;
        }

        if (!string.IsNullOrEmpty(request.NewGroupName))
        {
            await VideoStreams
            .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
               .ExecuteUpdateAsync(s => s.SetProperty(b => b.User_Tvg_group, request.NewGroupName), cancellationToken: cancellationToken)
               .ConfigureAwait(false);

            channelGroup.Name = request.NewGroupName;
            isChanged = true;
        }

        if (!string.IsNullOrEmpty(request.Regex))
        {
            channelGroup.RegexMatch = request.Regex;
        }

        ChannelGroups.Update(channelGroup);
        await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        ChannelGroupDto cgresult = _mapper.Map<ChannelGroupDto>(channelGroup);

        if (!isChanged)
        {
            return (cgresult, null, null);
        }

        List<VideoStreamDto> afterResults = VideoStreams
           .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
        .AsNoTracking()
           .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider).ToList();

        var afterRegexStreams = await GetVideoStreamsByNamePatternAsync(channelGroup.RegexMatch, cancellationToken).ConfigureAwait(false);
        if (afterRegexStreams != null)
        {
            var mapped = _mapper.Map<List<VideoStreamDto>>(afterRegexStreams);
            afterResults.AddRange(mapped);
        }

        List<VideoStreamDto> distinctList = beforeResults ?? new List<VideoStreamDto>();

        if (afterResults is not null)
        {
            var existingIds = new HashSet<string>(distinctList.Select(a => a.Id));
            var diff = afterResults.Where(a => !existingIds.Contains(a.Id));
            distinctList = distinctList.Concat(diff).ToList();
        }

        List<StreamGroupDto> streamGroups = new();
        if (distinctList.Any())
        {
            var intStreamGroups = await GetStreamGroupsByVideoStreamIdsAsync(distinctList.Select(a => a.Id).ToList(), url, cancellationToken).ConfigureAwait(false);
            if (intStreamGroups != null && intStreamGroups.Any())
            {
                foreach (var streamGroup in intStreamGroups)
                {
                    UpdateStreamGroupRequest updateStreamGroupRequest = new(
                        StreamGroupId: streamGroup.Id,
                        Name: null,
                        StreamGroupNumber: null,
                        VideoStreams: null,
                        ChannelGroupNames: streamGroup.ChannelGroups.Select(a => a.Name).ToList()
                        );

                    var res = await UpdateStreamGroupAsync(updateStreamGroupRequest, url, cancellationToken);
                    if (res is not null)
                    {
                        streamGroups.Add(res);
                    }
                }
            }
        }

        return (cgresult, distinctList, streamGroups);
    }
}
