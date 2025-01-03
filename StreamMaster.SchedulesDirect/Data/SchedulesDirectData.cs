namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData(int EPGNumber) : ISchedulesDirectData
{
    public int EPGNumber { get; set; } = EPGNumber;

    public void ResetLists()
    {
        Lineups.Clear();

        Services.Clear();
    }
}
