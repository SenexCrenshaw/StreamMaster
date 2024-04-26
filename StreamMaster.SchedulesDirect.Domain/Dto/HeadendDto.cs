﻿using StreamMaster.Domain.Mappings;

namespace StreamMaster.SchedulesDirect.Domain.Dto;

public class HeadendDto : IMapFrom<Headend>
{
    public string Id => HeadendId + "|" + Lineup;
    public string HeadendId { get; set; }
    public string Lineup { get; set; }
    public string Location { get; set; }
    public string Name { get; set; }
    public string Transport { get; set; }

}