using FluentValidation;

using MediatR;

using StreamMasterApplication.VideoStreams.Events;

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

public class DeleteM3UFileHandler : IRequestHandler<DeleteM3UFileRequest, int?>
{
    private readonly IAppDbContext _context;
    private readonly IPublisher _publisher;

    public DeleteM3UFileHandler(IPublisher publisher,
         IAppDbContext context
        )
    {
        _publisher = publisher;
        _context = context;
    }

    public async Task<int?> Handle(DeleteM3UFileRequest request, CancellationToken cancellationToken = default)
    {
        M3UFile? m3UFile = await _context.M3UFiles.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (m3UFile == null)
        {
            return null;
        }

        _ = _context.M3UFiles.Remove(m3UFile);

        if (request.DeleteFile)
        {
            string fullName = Path.Combine(FileDefinitions.M3U.DirectoryLocation, m3UFile.Name + FileDefinitions.M3U.FileExtension);
            if (File.Exists(fullName))
            {
                FileAttributes attributes = File.GetAttributes(fullName);

                if ((attributes & (FileAttributes.ReadOnly | FileAttributes.System)) != 0)
                {
                }else
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

        var targetM3UFileIdGroups = _context.VideoStreams
            .Where(vs => vs.M3UFileId == m3UFile.Id)
            .Select(vs => vs.Tvg_group);

        var otherM3UFileIdGroups = _context.VideoStreams
            .Where(vs => vs.M3UFileId != m3UFile.Id)
            .Select(vs => vs.Tvg_group);

        var groupsToDelete = targetM3UFileIdGroups.Except(otherM3UFileIdGroups).ToList();

        foreach (var gtd in groupsToDelete)
        {
            var group = _context.ChannelGroups.Where(tg => tg.Name == gtd).FirstOrDefault();
            if (group != null)
            {
                _context.ChannelGroups.Remove(group);
            }
        }

        var streams = _context.VideoStreams.Where(a => a.M3UFileId == m3UFile.Id);
        _context.VideoStreams.RemoveRange(streams);

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await _publisher.Publish(new M3UFileDeletedEvent(m3UFile.Id), cancellationToken).ConfigureAwait(false);

        foreach (var stream in streams)
        {
            await _publisher.Publish(new DeleteVideoStreamEvent(stream.Id), cancellationToken).ConfigureAwait(false);
        }

        return m3UFile.Id;
    }
}
