using StreamMaster.SchedulesDirect.Domain.Helpers;
using StreamMaster.SchedulesDirect.Domain.Models;

namespace StreamMaster.Domain.Extensions;

public static class MxfServiceExtensions
{
    public static MxfService? GetMxfService(this IEnumerable<MxfService> services, string mxfServiceName)
    {
        MxfService? origService = services.FirstOrDefault(a => a.StationId == mxfServiceName);
        if (origService == null)
        {
            if (!EPGChecks.IsValidEPGId(mxfServiceName))
            {
                string toTest = $"-{mxfServiceName}";
                origService = services.OrderBy(a => a.EPGNumber).FirstOrDefault(a => a.StationId.Contains(toTest, StringComparison.OrdinalIgnoreCase));

            }

        }

        return origService;
    }
}
