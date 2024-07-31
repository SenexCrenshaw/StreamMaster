namespace StreamMaster.Application.Interfaces;

public interface IStreamGroupService
{
    Task<StreamGroup?> GetStreamGroupFromIdAsync(int streamGroupId);
    Task<StreamGroup?> GetStreamGroupFromNameAsync(string streamGroupName);
    Task<StreamGroup> GetDefaultSGAsync();
    Task<int> GetDefaultSGIdAsync();
}