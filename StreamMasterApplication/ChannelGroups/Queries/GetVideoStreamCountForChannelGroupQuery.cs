using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetVideoStreamCountForChannelGroup(string channelGropupName) : IRequest<GetVideoStreamCountForChannelGroupResponse>;

internal class GetVideoStreamCountForChannelGroupHandler : BaseMediatorRequestHandler, IRequestHandler<GetVideoStreamCountForChannelGroup, GetVideoStreamCountForChannelGroupResponse>
{
    public GetVideoStreamCountForChannelGroupHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<GetVideoStreamCountForChannelGroupResponse> Handle(GetVideoStreamCountForChannelGroup request, CancellationToken cancellationToken)
    {
        var res = new GetVideoStreamCountForChannelGroupResponse { ActiveCount = 0, HiddenCount = 0 };
        var cg = await Sender.Send(new GetChannelGroupByName(request.channelGropupName), cancellationToken).ConfigureAwait(false);
        if (cg == null)
        {
            return res;
        }

        List<string> ids = new List<string>();
        IEnumerable<VideoStream> reg;

        var t = await Repository.VideoStream.GetVideoStreamsChannelGroupName(request.channelGropupName);
        ids = t.Select(a => a.Id).ToList();
        var hiddenCount = t.Where(a => a.IsHidden).Count();

        if (!string.IsNullOrEmpty(cg.RegexMatch))
        {
            reg = await Sender.Send(new GetVideoStreamsByNamePatternQuery(cg.RegexMatch), cancellationToken).ConfigureAwait(false);
            hiddenCount += reg.Where(a => a.IsHidden && !ids.Contains(a.Id)).Count();
            ids.AddRange(t.Select(a => a.Id));
            ids = ids.Distinct().ToList();
        }

        res.TotalCount = ids.Count;
        res.ActiveCount = ids.Count - hiddenCount;

        res.HiddenCount = hiddenCount;
        res.Id = cg.Id;
        return res;
    }
}