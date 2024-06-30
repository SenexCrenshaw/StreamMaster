﻿using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class VideoStreamConfig : SMChannel
{
    public bool IsDuplicate { get; set; }
    public bool IsDummy { get; set; }
}


//foreach (VideoStream dummy in dummies)
//{
//    string dummyName = "DUMMY-" + dummy.Id;

//    MxfService mxfService = schedulesDirectData.FindOrCreateService(dummyName);
//    mxfService.CallSign = dummy.User_Tvg_name;
//    mxfService.Name = dummy.User_Tvg_name;

//    MxfLineup mxfLineup = schedulesDirectData.FindOrCreateLineup($"ZZZ-{dummyName}-StreamMaster", $"ZZZSM {dummyName} Lineup");
//    mxfLineup.channels.Add(new MxfChannel(mxfLineup, mxfService));
//}




//if (videoStreamConfig.IsDuplicate)
//{
//    MxfService? newService = schedulesDirectData.Services.FirstOrDefault(a => a.StationId == videoStreamConfig.User_Tvg_ID);
//    if (newService is not null)
//    {
//        continue;
//    }

//    string[] parts = stationId.Split('-');
//    string userTvgId = parts[1];
//    string userTvgName = parts[2];

//    MxfService? origService = schedulesDirectData.Services.FirstOrDefault(a => a.StationId == userTvgId);
//    if (origService is null)
//    {
//        continue;
//    }
