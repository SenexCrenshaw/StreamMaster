using StreamMaster.Application.SMStreams.Commands;

namespace StreamMaster.Application.Interfaces;

public interface IStreamMasterHub
{
    Task AuthLogOut();
    Task IsSystemReady(bool isSystemReady);
    Task TaskIsRunning(bool isSystemReady);
    Task SetField(List<FieldData> fieldData);
    Task ClearByTag(ClearByTag result);
    Task DataRefresh(string entityName);
    Task SendStatus(ImageDownloadServiceStatus status);
    Task SendMessage(SMMessage smMessage);
    Task SendSMTasks(List<SMTask> smTask);
}