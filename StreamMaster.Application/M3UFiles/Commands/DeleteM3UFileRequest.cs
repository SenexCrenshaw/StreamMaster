using FluentValidation;

namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteM3UFileRequest(bool DeleteFile, int Id) : IRequest<APIResponse>;

public class DeleteM3UFileRequestHandler(ILogger<DeleteM3UFileRequest> logger, IFileUtilService fileUtilService, IM3UFileService m3UFileService, ICacheManager CacheManager, IMessageService messageService, IDataRefreshService dataRefreshService, ILogoService logoService, IRepositoryWrapper Repository)
    : IRequestHandler<DeleteM3UFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteM3UFileRequest request, CancellationToken cancellationToken = default)
    {
        M3UFile? m3UFile = await Repository.M3UFile.GetM3UFileAsync(request.Id).ConfigureAwait(false);
        if (m3UFile == null)
        {
            await messageService.SendError("M3U file not found");
            return APIResponse.NotFound;
        }

        try
        {
            await Repository.M3UFile.DeleteM3UFile(m3UFile.Id);

            if (request.DeleteFile)
            {
                string? fullName = m3UFileService.GetFileName(m3UFile.Name).fullName;
                fileUtilService.CleanUpFile(fullName);
            }

            IQueryable<SMStream> smStreams = Repository.SMStream.GetQuery();

            IQueryable<string> targetM3UFileIdGroups = smStreams
                .Where(vs => vs.M3UFileId == m3UFile.Id)
                .Select(vs => vs.Group).Distinct();

            IQueryable<string> otherM3UFileIdGroups = smStreams
                .Where(vs => vs.M3UFileId != m3UFile.Id)
                .Select(vs => vs.Group);

            List<string> groupsToDelete = [.. targetM3UFileIdGroups.Except(otherM3UFileIdGroups)];
            if (groupsToDelete.Count > 0)
            {
                await Repository.ChannelGroup.DeleteChannelGroupsByNameRequest(groupsToDelete);
            }

            await Repository.SMStream.DeleteSMStreamsByM3UFiledId(m3UFile.Id, cancellationToken);
            await Repository.SaveAsync().ConfigureAwait(false);

            CacheManager.M3UMaxStreamCounts.TryRemove(m3UFile.Id, out _);

            logoService.RemoveLogosByM3UFileId(m3UFile.Id);

            await dataRefreshService.RefreshAllM3U();
            if (groupsToDelete.Count > 0)
            {
                await dataRefreshService.RefreshChannelGroups();
            }
            await messageService.SendSuccess("Deleted '" + m3UFile.Name + "'", "Delete M3U");

            return APIResponse.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteM3UFileRequest {request}", request);
            await messageService.SendError("Exception deleting M3U", ex.Message);
            return APIResponse.NotFound;
        }
    }
}