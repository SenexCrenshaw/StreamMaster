namespace StreamMaster.SchedulesDirect.Domain.Models;

public class VideoStreamConfig
{
    public int M3UFileId { get; set; }
    public string Id { get; set; } = string.Empty;
    public string User_Tvg_name { get; set; } = string.Empty;
    public string User_Tvg_ID { get; set; } = string.Empty;
    public string User_Tvg_Logo { get; set; } = string.Empty;
    public int User_Tvg_chno { get; set; }
    public bool IsDuplicate { get; set; }
    public bool IsDummy { get; set; }
    public string Tvg_ID { get; set; }
    public int TimeShift { get; set; }
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
