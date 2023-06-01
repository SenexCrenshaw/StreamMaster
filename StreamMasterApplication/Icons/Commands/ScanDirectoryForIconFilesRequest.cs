using MediatR;

using Microsoft.EntityFrameworkCore;

namespace StreamMasterApplication.Icons.Commands;

public class ScanDirectoryForIconFilesRequest : IRequest<bool>
{
}

public class ScanDirectoryForIconFilesRequestHandler : IRequestHandler<ScanDirectoryForIconFilesRequest, bool>
{
    private readonly IAppDbContext _context;
    
    public ScanDirectoryForIconFilesRequestHandler(         IAppDbContext context)    {
        _context = context;
    }

    public async Task<bool> Handle(ScanDirectoryForIconFilesRequest command, CancellationToken cancellationToken)
    {
        FileDefinition fd = FileDefinitions.Icon;

        DirectoryInfo iconDirInfo = new(fd.DirectoryLocation);

        EnumerationOptions er = new()
        {
            MatchCasing = MatchCasing.CaseInsensitive
        };

        string[] extensions = fd.FileExtension.Split('|');
        int count = 0;
        foreach (FileInfo fileInfo in iconDirInfo.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (fileInfo.DirectoryName == null)
            {
                continue;
            }

            string txtName = Path.Combine(fileInfo.DirectoryName, Path.GetFileNameWithoutExtension(fileInfo.FullName) + ".url");

            if (!File.Exists(txtName))
            {
                continue;
            }

            string originalSource = File.ReadAllText(txtName);
            ++count;

            if ( _context.Icons.Where(a => a.FileExists).ToList().Any(a => a.OriginalSource.Equals(originalSource)))
            {
                continue;
            }

            string ext = Path.GetExtension(fileInfo.Name);
            if (ext.StartsWith('.'))
            {
                ext = ext[1..];
            }
            fd.FileExtension = ext;
            IconFile icon = new()
            {
                OriginalSource = originalSource,
                Source = originalSource,
                Url = originalSource,
                Name = Path.GetFileNameWithoutExtension(fileInfo.Name),
                ContentType = $"image/{ext}",
                LastDownloaded = DateTime.Now,
                LastDownloadAttempt = DateTime.Now,
                FileExists = true,
                MetaData = "",
                FileExtension = ext
            };

            _ = _context.Icons.Add(icon);

            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        // await _publisher.Publish(new IconFileAddedEvent(icon), cancellationToken).ConfigureAwait(false);
        return true;
    }
}