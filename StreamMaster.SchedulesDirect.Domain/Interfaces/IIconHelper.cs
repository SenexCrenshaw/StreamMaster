using StreamMaster.Domain.Enums;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IIconHelper
    {
        string GetIconUrl(int EPGNumber, string iconOriginalSource, string _baseUrl, SMFileTypes? sMFileTypes = null);
    }
}