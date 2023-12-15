using FluentValidation;

using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.M3UFiles.Commands;

public record DeleteM3UFileRequest(bool DeleteFile, int Id) : IRequest<int?> { }

public class DeleteM3UFileRequestValidator : AbstractValidator<DeleteM3UFileRequest>
{
    public DeleteM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class DeleteM3UFileRequestHandler(ILogger<DeleteM3UFileRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<DeleteM3UFileRequest, int?>
{
    public async Task<int?> Handle(DeleteM3UFileRequest request, CancellationToken cancellationToken = default)
    {
        M3UFile? m3UFile = await Repository.M3UFile.GetM3UFileById(request.Id).ConfigureAwait(false);
        if (m3UFile == null)
        {
            return null;
        }
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
            }
            else
            {
                //_logger.LogError("DeleteEPGFile File {fulleName} does not exist", fulleName);
            }
        }

        IQueryable<VideoStream> videoStreams = Repository.VideoStream.GetVideoStreamQuery();

        IQueryable<string> targetM3UFileIdGroups = videoStreams
            .Where(vs => vs.M3UFileId == m3UFile.Id)
            .Select(vs => vs.Tvg_group).Distinct();

        IQueryable<string> otherM3UFileIdGroups = videoStreams
            .Where(vs => vs.M3UFileId != m3UFile.Id)
            .Select(vs => vs.Tvg_group);

        List<string> groupsToDelete = targetM3UFileIdGroups.Except(otherM3UFileIdGroups).ToList();

        foreach (string? gtd in groupsToDelete)
        {
            ChannelGroup? group = await Repository.ChannelGroup.GetChannelGroupByName(gtd).ConfigureAwait(false);
            if (group != null)
            {
                await Repository.ChannelGroup.DeleteChannelGroupById(group.Id);
                _ = await Repository.SaveAsync().ConfigureAwait(false);
            }
        }

        List<VideoStreamDto> streams = await Repository.VideoStream.DeleteVideoStreamsByM3UFiledId(m3UFile.Id, cancellationToken);

        List<IconFileDto> icons = MemoryCache.Icons();
        _ = icons.RemoveAll(a => a.FileId == m3UFile.Id);
        MemoryCache.SetIcons(icons);

        _ = await Repository.SaveAsync().ConfigureAwait(false);

        await Publisher.Publish(new M3UFileDeletedEvent(m3UFile.Id), cancellationToken).ConfigureAwait(false);

        await Publisher.Publish(new DeleteVideoStreamsEvent(streams.Select(a => a.Id).ToList()), cancellationToken).ConfigureAwait(false);

        return m3UFile.Id;
    }
}
