namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetHeadendsRequest(string country, string postalCode) : IRequest<DataResponse<List<HeadendDto>>>;

internal class GetHeadendsRequestHandler(ISchedulesDirect schedulesDirect)
    : IRequestHandler<GetHeadendsRequest, DataResponse<List<HeadendDto>>>
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

    public async Task<DataResponse<List<HeadendDto>>> Handle(GetHeadendsRequest request, CancellationToken cancellationToken)
    {
        var result = await schedulesDirect.GetHeadends(request.country, request.postalCode, cancellationToken);
        if (result == null)
        {
            return DataResponse<List<HeadendDto>>.ErrorWithMessage("Request failed");
        }

        var ret = MapFrom(result);

        return DataResponse<List<HeadendDto>>.Success(ret);
    }
}
