using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Filtering;
using StreamMasterDomain.Pagination;

using System.Text.Json;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamsForChannelGroups(VideoStreamParameters VideoStreamParameters) : IRequest<PagedResponse<VideoStreamDto>>;

internal class GetVideoStreamsForChannelGroupsHandler : BaseMediatorRequestHandler, IRequestHandler<GetVideoStreamsForChannelGroups, PagedResponse<VideoStreamDto>>
{
    public GetVideoStreamsForChannelGroupsHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<PagedResponse<VideoStreamDto>?> Handle(GetVideoStreamsForChannelGroups request, CancellationToken cancellationToken)
    {
        var results = new List<VideoStreamDto>();
        List<string>? channelGroupNames = null;

        if (!string.IsNullOrEmpty(request.VideoStreamParameters.JSONArgumentString))
        {
            channelGroupNames = JsonSerializer.Deserialize<List<string>>(request.VideoStreamParameters.JSONArgumentString);
        }
        else
        {
            var ret = await Sender.Send(new GetVideoStreams(request.VideoStreamParameters), cancellationToken).ConfigureAwait(false);
            return ret;
        }

        if (channelGroupNames == null)
        {
            return null;
        }

        List<string> ids = new List<string>();

        foreach (var cgName in channelGroupNames)
        {
            var videoStreams = await Repository.VideoStream.GetVideoStreamsChannelGroupName(cgName);
            var toAdd = Mapper.Map<List<VideoStreamDto>>(videoStreams.Where(a => !ids.Contains(a.Id)));
            ids.AddRange(toAdd.Select(a => a.Id));
            results.AddRange(toAdd);
        }

        var pagedResult = await results.ToPagedListAsync(request.VideoStreamParameters.PageNumber, request.VideoStreamParameters.PageSize).ConfigureAwait(false);

        var pagedResponse = pagedResult.ToPagedResponse(ids.Count());

        return pagedResponse;
    }
}