using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.VideoStreams.Events;
using StreamMasterDomain.Cache;

namespace StreamMasterApplication.M3UFiles.Commands;

public class DeleteM3UFileRequest : IRequest<int?>
{
    public bool DeleteFile { get; set; }
    public int Id { get; set; }
}

public class DeleteM3UFileRequestValidator : AbstractValidator<DeleteM3UFileRequest>
{
    public DeleteM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class DeleteM3UFileHandler : BaseMemoryRequestHandler, IRequestHandler<DeleteM3UFileRequest, int?>
{

    public DeleteM3UFileHandler(ILogger<ProcessM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<int?> Handle(DeleteM3UFileRequest request, CancellationToken cancellationToken = default)
    {
        M3UFile? m3UFile = await Repository.M3UFile.GetM3UFileByIdAsync(request.Id).ConfigureAwait(false);
        if (m3UFile == null)
        {
            return null;
        }
        Repository.M3UFile.DeleteM3UFile(m3UFile);


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

                string txtName = Path.Combine(FileDefinitions.M3U.DirectoryLocation, Path.GetFileNameWithoutExtension(m3UFile.Source) + ".url");
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

        IQueryable<VideoStream> videoStreams = Repository.VideoStream.GetAllVideoStreams();

        IQueryable<string> targetM3UFileIdGroups = videoStreams
            .Where(vs => vs.M3UFileId == m3UFile.Id)
            .Select(vs => vs.Tvg_group);

        IQueryable<string> otherM3UFileIdGroups = videoStreams
            .Where(vs => vs.M3UFileId != m3UFile.Id)
            .Select(vs => vs.Tvg_group);

        List<string> groupsToDelete = targetM3UFileIdGroups.Except(otherM3UFileIdGroups).ToList();

        foreach (string? gtd in groupsToDelete)
        {
            ChannelGroup? group = Repository.ChannelGroup.GetAllChannelGroups().Where(tg => tg.Name == gtd).FirstOrDefault();
            if (group != null)
            {
                Repository.ChannelGroup.DeleteChannelGroup(group);
                await Repository.SaveAsync().ConfigureAwait(false);
            }
        }

        IEnumerable<VideoStream> streams = await Repository.VideoStream.DeleteVideoStreamsByM3UFiledId(m3UFile.Id, cancellationToken);

        List<StreamMasterDomain.Dto.IconFileDto> icons = MemoryCache.Icons();
        icons.RemoveAll(a => a.FileId == m3UFile.Id);
        MemoryCache.Set(icons);

        await Repository.SaveAsync().ConfigureAwait(false);

        await Publisher.Publish(new M3UFileDeletedEvent(m3UFile.Id), cancellationToken).ConfigureAwait(false);

        await Publisher.Publish(new DeleteVideoStreamsEvent(streams.Select(a => a.Id).ToList()), cancellationToken).ConfigureAwait(false);

        return m3UFile.Id;
    }
}
