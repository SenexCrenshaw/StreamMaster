namespace StreamMaster.Domain.Enums;

[TsEnum]

public enum JobType
{
    ProcessM3U,
    RefreshM3U,
    TimerM3U,
    ProcessEPG,
    RefreshEPG,
    UpdateEPG,
    UpdateM3U,
    TimerEPG,
    SDSync,
    Backup,
    TimerBackup,
    EPGRemovedExpiredKeys
}