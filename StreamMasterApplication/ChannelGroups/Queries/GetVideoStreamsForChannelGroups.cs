using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetVideoStreamsForChannelGroups(VideoStreamParameters VideoStreamParameters) : IRequest<PagedResponse<VideoStreamDto>>;

internal class GetVideoStreamsForChannelGroupsHandler : BaseMediatorRequestHandler, IRequestHandler<GetVideoStreamsForChannelGroups, PagedResponse<VideoStreamDto>>
{
    public GetVideoStreamsForChannelGroupsHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<PagedResponse<VideoStreamDto>> Handle(GetVideoStreamsForChannelGroups request, CancellationToken cancellationToken)
    {

        PagedResponse<VideoStreamDto> ret = await Sender.Send(new GetVideoStreams(request.VideoStreamParameters), cancellationToken).ConfigureAwait(false);
        return ret;
        //List<VideoStreamDto> results = new();
        //List<string> channelGroupNames = null;

        //if (!string.IsNullOrEmpty(request.VideoStreamParameters.JSONArgumentString))
        //{
        //    channelGroupNames = JsonSerializer.Deserialize<List<string>>(request.VideoStreamParameters.JSONArgumentString);
        //}
        //else
        //{
        //    PagedResponse<VideoStreamDto> ret = await Sender.Send(new GetVideoStreams(request.VideoStreamParameters), cancellationToken).ConfigureAwait(false);
        //    return ret;
        //}

        //if (channelGroupNames == null)
        //{
        //    return null;
        //}

        //List<string> ids = new();

        //foreach (string cgName in channelGroupNames)
        //{
        //    PagedResponse<VideoStreamDto> videoStreams = await Repository.VideoStream.GetVideoStreamsChannelGroupName(request.VideoStreamParameters, cgName, cancellationToken);
        //    //List<VideoStreamDto> toAdd = Mapper.Map<List<VideoStreamDto>>(videoStreams.Where(a => !ids.Contains(a.Id)));
        //    ids.AddRange(videoStreams.Data.Select(a => a.Id));
        //    results.AddRange(videoStreams.Data);
        //}

        //IPagedList<VideoStreamDto> pagedResult = await results.ToPagedListAsync(request.VideoStreamParameters.PageNumber, request.VideoStreamParameters.PageSize).ConfigureAwait(false);

        //PagedResponse<VideoStreamDto> pagedResponse = pagedResult.ToPagedResponse(ids.Count());

        //return pagedResponse;
    }
}