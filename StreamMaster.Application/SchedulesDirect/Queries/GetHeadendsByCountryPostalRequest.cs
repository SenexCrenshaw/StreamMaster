namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetHeadendsByCountryPostalRequest(string Country, string PostalCode) : IRequest<DataResponse<List<HeadendDto>>>;

internal class GetHeadendsByCountryPostalRequestHandler(ISchedulesDirectAPIService schedulesDirectAPIService)
    : IRequestHandler<GetHeadendsByCountryPostalRequest, DataResponse<List<HeadendDto>>>
{
    private static List<HeadendDto> MapFrom(List<Headend> headends, string Country, string PostalCode)
    {
        //var result = await lineups.GetLineups(CancellationToken.None);
        //var subscribedLineups = result.Select(x => x.Lineup).ToList();
        List<HeadendDto> ret = [];
        foreach (Headend headend in headends)
        {
            foreach (HeadendLineup lineup in headend.Lineups)
            {
                HeadendDto headendDto = new()
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
        List<Headend>? result = await schedulesDirectAPIService.GetHeadendsByCountryPostalAsync(request.Country, request.PostalCode, cancellationToken);
        if (result == null)
        {
            return DataResponse<List<HeadendDto>>.ErrorWithMessage("Request failed");
        }

        List<HeadendDto> ret = MapFrom(result, request.Country, request.PostalCode);

        return DataResponse<List<HeadendDto>>.Success(ret);
    }
}
