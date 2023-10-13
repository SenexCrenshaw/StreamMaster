using StreamMaster.SchedulesDirectAPI;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetHeadends(string country, string postalCode) : IRequest<List<HeadendDto>>;

internal class GetHeadendsHandler(ISettingsService settingsService) : IRequestHandler<GetHeadends, List<HeadendDto>>
{
    public async Task<List<HeadendDto>> Handle(GetHeadends request, CancellationToken cancellationToken)
    {
        List<HeadendDto> ret = new();
        Setting setting = await settingsService.GetSettingsAsync();
        SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
        StreamMaster.SchedulesDirectAPI.Models.SDStatus? status = await sd.GetStatus(cancellationToken).ConfigureAwait(false);
        if (status == null || !status.systemStatus.Any())
        {
            Console.WriteLine("Status is null");
            return ret;
        }

        SDSystemstatus systemStatus = status.systemStatus[0];
        if (systemStatus.status == "Offline")
        {
            Console.WriteLine($"Status is {systemStatus.status}");
            return ret;
        }

        List<StreamMaster.SchedulesDirectAPI.Models.Headend>? headends = await sd.GetHeadends(request.country, request.postalCode, cancellationToken).ConfigureAwait(false);
        foreach (StreamMaster.SchedulesDirectAPI.Models.Headend headend in headends)
        {
            //if (headend.lineups.Count() > 1)
            //{
            //    continue;
            //}
            foreach (StreamMaster.SchedulesDirectAPI.Models.Lineup lineup in headend.lineups)
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
