namespace StreamMaster.Domain.Enums;

public enum JobType
{
    ProcessM3U,
    RefreshM3U,
    TimerM3U,
    ProcessEPG,
    RefreshEPG,
    UpdateEPG,
    TimerEPG,
    SDSync,
    Backup,
    TimerBackup
}