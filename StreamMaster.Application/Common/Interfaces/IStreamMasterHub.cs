using StreamMaster.Application.SMStreams.Commands;

namespace StreamMaster.Application.Common.Interfaces;

public interface IStreamMasterHub //: ISharedHub
{
    Task IsSystemReady(bool isSystemReady);
    Task TaskIsRunning(bool isSystemReady);
    Task SetField(List<FieldData> fieldData);
    Task ClearByTag(ClearByTag result);
    Task DataRefresh(string entityName);
    Task SendMessage(SMMessage smMessage);
    Task SendSMTasks(List<SMTask> smTask);
}