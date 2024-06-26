namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetHeadendsByCountryPostalRequest(string Country, string PostalCode) : IRequest<DataResponse<List<HeadendDto>>>;

internal class GetHeadendsByCountryPostalRequestHandler(ISchedulesDirect schedulesDirect)
    : IRequestHandler<GetHeadendsByCountryPostalRequest, DataResponse<List<HeadendDto>>>
{

    private async Task<List<HeadendDto>> MapFrom(List<Headend> headends, string Country, string PostalCode)
    {
        //var result = await lineups.GetLineups(CancellationToken.None);
        //var subscribedLineups = result.Select(x => x.Lineup).ToList();
        var ret = new List<HeadendDto>();
        foreach (var headend in headends)
        {
            foreach (var lineup in headend.Lineups)
            {
                var headendDto = new HeadendDto
                {
                    Id = headend.HeadendId + "|" + lineup.Lineup,
                    HeadendId = headend.HeadendId,
                    Location = headend.Location,
                    Transport = headend.Transport,
                    Name = lineup.Name,
                    Lineup = lineup.Lineup,
                    Country = Country,
                    PostCode = PostalCode
                    //SubScribed = subscribedLineups.Contains(lineup.Lineup)
                };

                ret.Add(headendDto);
            }
        }

        return ret;
    }

    public async Task<DataResponse<List<HeadendDto>>> Handle(GetHeadendsByCountryPostalRequest request, CancellationToken cancellationToken)
    {
        var result = await schedulesDirect.GetHeadendsByCountryPostal(request.Country, request.PostalCode, cancellationToken);
        if (result == null)
        {
            return DataResponse<List<HeadendDto>>.ErrorWithMessage("Request failed");
        }

        var ret = await MapFrom(result, request.Country, request.PostalCode);

        return DataResponse<List<HeadendDto>>.Success(ret);
    }
}
