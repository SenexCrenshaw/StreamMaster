using StreamMaster.SchedulesDirectAPI.Domain.JsonClasses;

namespace StreamMaster.SchedulesDirectAPI;

public static class SDExtensions
{
    public static List<string>? CheckStatus(this UserStatus status)
    {
        List<string> ret = new();

        foreach (var lineup in status.Lineups)
        {
            if (lineup.IsDeleted)
            {
                ret.Add($"Lineup {lineup.Lineup} is deleted");
            }
        }

        return ret.Any() ? ret : null;
    }
   
}