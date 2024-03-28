using FluentValidation;
namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteM3UFileRequest(bool DeleteFile, int Id) : IRequest<DefaultAPIResponse> { }

public class DeleteM3UFileRequestHandler(ILogger<DeleteM3UFileRequest> logger, IMessageService messageService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IIconService iconService, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<DeleteM3UFileRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(DeleteM3UFileRequest request, CancellationToken cancellationToken = default)
    {
        M3UFile? m3UFile = await Repository.M3UFile.GetM3UFile(request.Id).ConfigureAwait(false);
        if (m3UFile == null)
        {
            await messageService.SendError("M3U file not found");
            return APIResponseFactory.NotFound;
        }

        try
        {
            bool refreshCGs = false;

            await Repository.M3UFile.DeleteM3UFile(m3UFile.Id);


            if (request.DeleteFile)
            {
                string fullName = Path.Combine(FileDefinitions.M3U.DirectoryLocation, m3UFile.Name + FileDefinitions.M3U.FileExtension);
                if (File.Exists(fullName))
                {
                    FileAttributes attributes = File.GetAttributes(fullName);

                    if ((attributes & (FileAttributes.ReadOnly | FileAttributes.System)) != 0)
                    {
                    }
                    else
                    {
                        File.Delete(fullName);
                    }

                    string txtName = Path.Combine(FileDefinitions.M3U.DirectoryLocation, Path.GetFileNameWithoutExtension(m3UFile.Source) + ".json");
                    if (File.Exists(txtName))
                    {
                        attributes = File.GetAttributes(txtName);
                        if ((attributes & (FileAttributes.ReadOnly | FileAttributes.System)) != 0)
                        {
                        }
                        else
                        {
                            File.Delete(txtName);
                        }
                    }
                    txtName = Path.Combine(FileDefinitions.M3U.DirectoryLocation, Path.GetFileNameWithoutExtension(m3UFile.Source) + ".url");
                    if (File.Exists(txtName))
                    {
                        attributes = File.GetAttributes(txtName);
                        if ((attributes & (FileAttributes.ReadOnly | FileAttributes.System)) != 0)
                        {
                        }
                        else
                        {
                            File.Delete(txtName);
                        }
                    }
                }
                else
                {
                    //_logger.LogError("DeleteEPGFile File {fulleName} does not exist", fulleName);
                }
            }

            IQueryable<SMStream> smStreams = Repository.SMStream.GetQuery();

            IQueryable<string> targetM3UFileIdGroups = smStreams
                .Where(vs => vs.M3UFileId == m3UFile.Id)
                .Select(vs => vs.Group).Distinct();

            IQueryable<string> otherM3UFileIdGroups = smStreams
                .Where(vs => vs.M3UFileId != m3UFile.Id)
                .Select(vs => vs.Group);

            List<string> groupsToDelete = targetM3UFileIdGroups.Except(otherM3UFileIdGroups).ToList();

            foreach (string? gtd in groupsToDelete)
            {
                ChannelGroup? group = await Repository.ChannelGroup.GetChannelGroupByName(gtd).ConfigureAwait(false);
                if (group != null)
                {
                    refreshCGs = true;
                    _ = await Repository.ChannelGroup.DeleteChannelGroupById(group.Id);
                }
            }

            await Repository.SMStream.DeleteSMStreamsByM3UFiledId(m3UFile.Id, cancellationToken);

            await Repository.SaveAsync().ConfigureAwait(false);

            iconService.RemoveIconsByM3UFileId(m3UFile.Id);

            await hubContext.Clients.All.DataRefresh("M3UFileDto").ConfigureAwait(false);
            await hubContext.Clients.All.DataRefresh("SMStreamDto").ConfigureAwait(false);
            await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);

            if (refreshCGs)
            {
                await hubContext.Clients.All.DataRefresh("ChannelGroupDto").ConfigureAwait(false);
            }

            await messageService.SendSuccess("Deleted M3U '" + m3UFile.Name);

            return APIResponseFactory.Ok;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteM3UFileRequest {request}", request);
            await messageService.SendError("Exception deleting M3U", ex.Message);
            return APIResponseFactory.NotFound;
        }
    }
}
