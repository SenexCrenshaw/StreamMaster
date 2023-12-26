namespace StreamMasterApplication.SchedulesDirect.Queries;

public record GetHeadends(string country, string postalCode) : IRequest<List<HeadendDto>>;

internal class GetHeadendsHandler(ISchedulesDirect schedulesDirect) : IRequestHandler<GetHeadends, List<HeadendDto>>
{

    private static List<HeadendDto> MapFrom(List<Headend> headends)
    {
        var ret = new List<HeadendDto>();
        foreach (var headend in headends)
        {
            foreach (var lineup in headend.Lineups)
            {
                var headendDto = new HeadendDto
                {
                    HeadendId = headend.HeadendId,
                    Location = headend.Location,
                    Transport = headend.Transport,
                    Name = lineup.Name,
                    Lineup = lineup.Lineup,
                };

                ret.Add(headendDto);
            }
        }

        return ret;
    }

    public async Task<List<HeadendDto>> Handle(GetHeadends request, CancellationToken cancellationToken)
    {
        var result = await schedulesDirect.GetHeadends(request.country, request.postalCode, cancellationToken);
        if (result == null)
        {
            return [];
        }

        var ret = MapFrom(result);

        return ret;
    }
}
