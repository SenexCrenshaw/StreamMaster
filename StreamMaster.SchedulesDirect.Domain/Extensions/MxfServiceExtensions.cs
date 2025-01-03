using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.Helpers;

namespace StreamMaster.SchedulesDirect.Domain.Extensions;

public static class MxfServiceExtensions
{
    public static MxfService? GetMxfService(this IEnumerable<MxfService> services, string mxfServiceName)
    {
        MxfService? origService = services.FirstOrDefault(a => a.StationId == mxfServiceName);
        if (origService == null)
        {
            if (!EPGHelper.IsValidEPGId(mxfServiceName))
            {
                string toTest = $"-{mxfServiceName}";
                origService = services.OrderBy(a => a.EPGNumber).FirstOrDefault(a => a.StationId?.ContainsIgnoreCase(toTest) == true);
            }
        }

        return origService;
    }
}
