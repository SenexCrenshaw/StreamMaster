using MediatR;

namespace StreamMaster.Application.Logos.CommandsOld;

public class ScanDirectoryForLogoFilesRequest : IRequest<bool>;

public class ScanDirectoryForLogoFilesRequestHandler : IRequestHandler<ScanDirectoryForLogoFilesRequest, bool>
{
    public Task<bool> Handle(ScanDirectoryForLogoFilesRequest command, CancellationToken cancellationToken)
    {
        return Task.FromResult(false);
        //FileDefinition fd = FileDefinitions.M3ULogo;

        //DirectoryInfo iconDirInfo = new(fd.DirectoryLocation);

        //EnumerationOptions er = new()
        //{
        //    MatchCasing = MatchCasing.CaseInsensitive
        //};

        //string[] extensions = fd.FileExtension.Split('|');
        //int count = 0;
        //foreach (FileInfo fileInfo in iconDirInfo.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())))
        //{
        //    if (cancellationToken.IsCancellationRequested)
        //    {
        //        break;
        //    }

        // if (fileInfo.DirectoryName == null) { continue; }

        // string filePath = Path.Combine(fileInfo.DirectoryName,
        // Path.GetFileNameWithoutExtension(fileInfo.FullName) + ".url");

        // string source = "";

        // if (FileUtil.ReadUrlFromFile(filePath, out string? url)) { source =
        // url; } else { source = fileInfo.ProfileName; }

        // ++count;

        // if (_context.Icons.Where(a => a.FileExists).ToList().Any(a =>
        // a.Source.Equals(source))) { continue; }

        // string ext = Path.GetExtension(fileInfo.ProfileName); if
        // (ext.StartsWith('.')) { ext = ext[1..]; } fd.FileExtension = ext;
        // IconFile icon = new() { Source = source, ProfileName =
        // Path.GetFileNameWithoutExtension(fileInfo.ProfileName), ContentType =
        // $"image/{ext}", LastDownloaded = DateTime.Now, LastDownloadAttempt =
        // DateTime.Now, FileExists = true, FileExtension = ext };

        // _ = _context.Icons.Add(icon);

        //    _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        //}

        //return true;
    }
}
