namespace StreamMaster.Domain.Services
{
    public interface IEPGHelper
    {
        //bool IsValidEPGId(string epgId);
        (int epgNumber, string stationId) ExtractEPGNumberAndStationId(string epgId);
        bool IsDummy(string epgId);
        bool IsDummy(int epgNumber);
        bool IsCustom(int epgNumber);
        bool IsSchedulesDirect(int epgNumber);
    }
}