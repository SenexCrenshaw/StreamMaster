using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamIdsByStreamGroupQuery(int streamGroupId) : IRequest<List<string>> { }

internal class GetVideoStreamIdsByStreamGroupQueryHandler : BaseRequestHandler, IRequestHandler<GetVideoStreamIdsByStreamGroupQuery, List<string>>
{

    public GetVideoStreamIdsByStreamGroupQueryHandler(ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }


    public async Task<List<string>> Handle(GetVideoStreamIdsByStreamGroupQuery request, CancellationToken cancellationToken)
    {

        // Fetch the stream group with associated channel groups
        StreamGroup? streamGroup = Repository.StreamGroup.GetAllStreamGroupsWithChannelGroups()
            .SingleOrDefault(sg => sg.Id == request.streamGroupId);
        if (streamGroup == null)
        {
            return new();
        }

        // Fetch all channel group names
        List<string> channelGroupNames = streamGroup.ChannelGroups.Select(cg => cg.ChannelGroup.Name).ToList();

        //// Fetch video stream IDs that match the user group
        List<string> matchedIds = Repository.VideoStream.GetAllVideoStreams()
            .Where(vs => channelGroupNames.Contains(vs.User_Tvg_group))
            .Select(vs => vs.Id)
            .ToList();

#if HAS_REGEX
  // Compile all regexes
        List<Regex> regexes = streamGroup.ChannelGroups
            .Where(a => !string.IsNullOrEmpty(a.ChannelGroup.RegexMatch))
            .Select(cg => new Regex(cg.ChannelGroup.RegexMatch, RegexOptions.ECMAScript | RegexOptions.IgnoreCase))
            .ToList();
        // If no regexes exist, return an empty list
        if (!regexes.Any() && !matchedIds.Any())
        {
            return new();
        }


        // Fetch all video streams
        IQueryable<VideoStream> allVideoStreams = Repository.VideoStream.GetAllVideoStreams();

        // Filter the video streams by matching names with regexes
        List<string> matchingVideoStreamIds = allVideoStreams
            .Where(vs => regexes.Any(regex => regex.IsMatch(vs.User_Tvg_name)))
            .Select(vs => vs.Id)
            .ToList();

        List<string> ret = matchingVideoStreamIds.Concat(matchedIds).Distinct().ToList();

        return ret;
#else 
        return matchedIds;
#endif
    }
}