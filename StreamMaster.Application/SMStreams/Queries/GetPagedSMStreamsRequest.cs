namespace StreamMaster.Application.SMStreams.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedSMStreamsRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<SMStreamDto>>;

internal class GetPagedSMStreamsRequestHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetPagedSMStreamsRequest, PagedResponse<SMStreamDto>>
{
    public async Task<PagedResponse<SMStreamDto>> Handle(GetPagedSMStreamsRequest request, CancellationToken cancellationToken)
    {
        if (request.Parameters.PageSize == 0)
        {
            return Repository.SMStream.CreateEmptyPagedResponse();
        }

        // Fetch the paged stream data
        PagedResponse<SMStreamDto> res = await Repository.SMStream
            .GetPagedSMStreams(request.Parameters, cancellationToken)
            .ConfigureAwait(false);

        if (res.Data is null)
        {
            return res;
        }

        // Get all relevant SMStreamIds from the response data
        List<string> ids = res.Data.ConvertAll(a => a.Id);

        // Fetch all the channelNameLogos in one go
        var channelNameLogos = await Repository.SMChannelStreamLink.GetQuery()
            .Where(a => ids.Contains(a.SMStreamId))
            .Include(a => a.SMChannel)
            .Select(a => new
            {
                a.SMStreamId,
                NameLogo = new NameLogo(a.SMStream)
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        // Group by SMStreamId for efficient mapping
        Dictionary<string, List<NameLogo>> groupedChannelNameLogos = channelNameLogos
            .GroupBy(a => a.SMStreamId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.NameLogo).OrderBy(nl => nl.Name).ToList());

        // Set Ids and assign the channel memberships to the corresponding streams
        foreach (SMStreamDto smStream in res.Data)
        {
            if (groupedChannelNameLogos.TryGetValue(smStream.Id, out List<NameLogo>? logos))
            {
                //for (int i = 0; i < logos.Count; i++)
                //{
                //    logos[i].Id = i.ToString();
                //}
                smStream.ChannelMembership = logos;
            }
            else
            {
                smStream.ChannelMembership = []; // Empty list if no memberships
            }
        }

        return res;
    }
}
