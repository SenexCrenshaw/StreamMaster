namespace StreamMaster.SchedulesDirectAPI.Helpers;

public static class SDHelpers
{
    public static SDStatus GetSDStatusOffline()
    {
        SDStatus ret = new();
        ret.systemStatus.Add(new SDSystemStatus { status = "Offline" });
        return ret;
    }
}
