using MediatR;

using StreamMaster.SchedulesDirectAPI;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetHeadends(string country, string postalCode) : IRequest<List<HeadendDto>>;

internal class GetHeadendsHandler : IRequestHandler<GetHeadends, List<HeadendDto>>
{
    public async Task<List<HeadendDto>> Handle(GetHeadends request, CancellationToken cancellationToken)
    {
        var ret = new List<HeadendDto>();
        Setting setting = FileUtil.GetSetting();

        var sd = new SchedulesDirect(setting.SDUserName, setting.SDPassword);
        var status = await sd.GetStatus(cancellationToken).ConfigureAwait(false);
        if (status == null || !status.systemStatus.Any())
        {
            Console.WriteLine("Status is null");
            return ret;
        }

        var systemStatus = status.systemStatus[0];
        if (systemStatus.status == "Offline")
        {
            Console.WriteLine($"Status is {systemStatus.status}");
            return ret;
        }

        var headends = await sd.GetHeadends(request.country, request.postalCode, cancellationToken).ConfigureAwait(false);
        foreach (var headend in headends)
        {
            //if (headend.lineups.Count() > 1)
            //{
            //    continue;
            //}
            foreach (var lineup in headend.lineups)
            {
                if (lineup.IsDeleted)
                {
                    continue;
                }
                ret.Add(new HeadendDto
                {
                    Headend = headend.headend,
                    Lineup = lineup.LineupString,
                    location = headend.location,
                    Name = lineup.Name,
                    Transport = headend.transport
                });
            }
        }

        return ret;
    }
}
