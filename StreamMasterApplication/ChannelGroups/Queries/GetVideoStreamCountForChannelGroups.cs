using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamCountForChannelGroups() : IRequest<IEnumerable<GetVideoStreamCountForChannelGroupResponse>>;

internal class GetVideoStreamCountForChannelGroupsHandler : BaseMediatorRequestHandler, IRequestHandler<GetVideoStreamCountForChannelGroups, IEnumerable<GetVideoStreamCountForChannelGroupResponse>>
{
    public GetVideoStreamCountForChannelGroupsHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<IEnumerable<GetVideoStreamCountForChannelGroupResponse>> Handle(GetVideoStreamCountForChannelGroups request, CancellationToken cancellationToken)
    {
        var results = new List<GetVideoStreamCountForChannelGroupResponse>();

        var cgs = Repository.ChannelGroup.GetAllChannelGroups();

        List<string> ids = new List<string>();

        foreach (var cg in cgs)
        {
            var res = new GetVideoStreamCountForChannelGroupResponse();
            IEnumerable<VideoStream> reg;

            var t = Repository.VideoStream.GetVideoStreamsChannelGroupName(cg.Name);
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
            res.ActiveCount = ids.Count- hiddenCount;            
            res.HiddenCount = hiddenCount;
            res.Id = cg.Id;
            results.Add(res);
        }
        return results;
    }
}