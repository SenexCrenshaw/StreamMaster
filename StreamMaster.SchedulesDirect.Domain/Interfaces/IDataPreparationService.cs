using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IDataPreparationService
    {
        //string BaseUrl { get; }
        //IReadOnlyDictionary<int, SeriesInfo> SeriesDict { get; }
        void AdjustServiceSchedules(MxfService service);
        List<MxfService> GetAllSdServices();
        OutputProfileDto GetDefaultOutputProfile();
        List<EPGFile> GetEpgFiles(List<VideoStreamConfig> videoStreamConfigs);
        SDSettings GetSdSettings();
        //List<MxfService> GetServicesToProcess(List<VideoStreamConfig> videoStreamConfigs);
        int GetTimeShift(VideoStreamConfig? videoStreamConfig, List<EPGFile> epgFiles);
        void Initialize(string baseUrl, List<VideoStreamConfig>? videoStreamConfigs);
    }
}