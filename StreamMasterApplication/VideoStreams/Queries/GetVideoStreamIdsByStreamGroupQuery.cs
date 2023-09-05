using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
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
        List<string> matchedIds = await Repository.VideoStream.GetAllVideoStreams()
            .Where(vs => channelGroupNames.Contains(vs.User_Tvg_group))
            .Select(vs => vs.Id)
            .ToListAsync(cancellationToken: cancellationToken);


        return matchedIds;

    }
}