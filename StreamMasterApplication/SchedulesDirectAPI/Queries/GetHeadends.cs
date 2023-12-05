namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetHeadends(string country, string postalCode) : IRequest<List<HeadendDto>>;

internal class GetHeadendsHandler(ISchedulesDirect schedulesDirect, IMapper mapper) : IRequestHandler<GetHeadends, List<HeadendDto>>
{

    public async Task<List<HeadendDto>> Handle(GetHeadends request, CancellationToken cancellationToken)
    {
        var result = await schedulesDirect.GetHeadends(request.country, request.postalCode, cancellationToken);

        var ret = mapper.Map<List<HeadendDto>>(result.OrderBy(a => a.HeadendId, StringComparer.OrdinalIgnoreCase).ToList());

        return ret;
    }
}
