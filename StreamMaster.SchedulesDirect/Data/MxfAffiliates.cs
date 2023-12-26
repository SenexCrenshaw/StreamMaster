
namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    private Dictionary<string, MxfAffiliate> _affiliates = [];
    public MxfAffiliate FindOrCreateAffiliate(string affiliateName)
    {
        if (_affiliates.TryGetValue(affiliateName, out MxfAffiliate? affiliate))
        {
            return affiliate;
        }

        Affiliates.Add(affiliate = new MxfAffiliate(affiliateName));
        _affiliates.Add(affiliateName, affiliate);
        return affiliate;
    }
}
