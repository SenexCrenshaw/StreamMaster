using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

public interface ISchedulesDirectDataService
{

    List<MxfService> GetAllServices { get; }
    List<MxfProgram> GetAllPrograms { get; }
    ISchedulesDirectData GetSchedulesDirectData(int ePGID);
    MxfService? GetService(string stationId);
}