using System.Runtime.CompilerServices;
using System.Web;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.Interfaces;
using StreamMaster.Application.StreamGroups;
using StreamMaster.Application.StreamGroups.Queries;
using StreamMaster.Domain.Extensions;
using StreamMaster.WebDav.Domain.Models;

namespace StreamMaster.WebDav.Providers;

public class VirtualStorageProvider(IServiceProvider serviceProvider) : IWebDavStorageProvider
{
    public async Task<string?> GetTSStreamUrlAsync(string path, CancellationToken cancellationToken)
    {
        if (!path.EndsWithIgnoreCase(".ts"))
        {
            return null;
        }

        path = HttpUtility.UrlDecode(path);
        string[] pathParts = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        IStreamGroupService streamGroupService = scope.ServiceProvider.GetRequiredService<IStreamGroupService>();

        Dictionary<int, SGFS> smfses = await streamGroupService.GetSMFS(null, true, cancellationToken);
        string sg = pathParts[0];
        string test = pathParts.Last();
        test = test[..^3];
        SGFS? sgfs = smfses.Values.FirstOrDefault(a => a.Name == sg);
        SMFile? smfs = sgfs.SMFS.FirstOrDefault(a => a.Name == test);
        return smfs?.Url;
    }

    public async Task<Stream?> GetFileAsync(string path, CancellationToken cancellationToken)
    {
        path = HttpUtility.UrlDecode(path);
        string[] pathParts = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        IStreamGroupService streamGroupService = scope.ServiceProvider.GetRequiredService<IStreamGroupService>();
        ISender Sender = scope.ServiceProvider.GetRequiredService<ISender>();

        Dictionary<int, SGFS> smfses = await streamGroupService.GetSMFS(null, true, cancellationToken);

        SGFS? sgfs = smfses.Values.FirstOrDefault(a => a.Name == GetLastPathSegment(pathParts[0]));
        if (sgfs is null)
        {
            return null;
        }

        if (path.EndsWithIgnoreCase(".xml"))
        {
            string xml = await Sender.Send(new GetStreamGroupEPG(2), cancellationToken).ConfigureAwait(false);
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
        }
        else if (path.EndsWithIgnoreCase(".m3u"))
        {
            string m3u = await Sender.Send(new GetStreamGroupM3U(2, true), cancellationToken).ConfigureAwait(false);
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(m3u));
        }
        else if (path.EndsWithIgnoreCase(".strm"))
        {
            return GenerateStrmStream(sgfs.Url);
        }

        return null;
    }

    public async IAsyncEnumerable<DirectoryEntry> ListDirectoryAsync(string path, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        IStreamGroupService streamGroupService = scope.ServiceProvider.GetRequiredService<IStreamGroupService>();

        Dictionary<int, SGFS> smfses = await streamGroupService.GetSMFS(null, true, cancellationToken);

        if (path.StartsWith("/webdav/"))
        {
            path = path[7..];
        }
        // Extract stream group ID from path
        path = HttpUtility.UrlDecode(path);
        string[] pathParts = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        bool isSubdirectory = pathParts.Length > 0;

        // Top-level: return stream groups
        if (!isSubdirectory)
        {
            foreach (KeyValuePair<int, SGFS> smfs in smfses)
            {
                yield return new DirectoryEntry
                {
                    Name = smfs.Value.Name,
                    Path = $"{smfs.Value.Name}/",
                    IsDirectory = true
                };
            }
            yield break;
        }

        SGFS? sgfs = smfses.Values.FirstOrDefault(a => a.Name == GetLastPathSegment(pathParts[0]));

        if (sgfs is not null)
        {
            if (pathParts.Length == 1)
            {
                // Root of stream group: Add .m3u, .xml, and ts/strm directories
                yield return new DirectoryEntry
                {
                    Name = "strm",
                    Path = $"{path}strm",
                    IsDirectory = true
                };
                yield return new DirectoryEntry
                {
                    Name = "ts",
                    Path = $"{path}ts",
                    IsDirectory = true
                };

                yield return new DirectoryEntry
                {
                    Name = $"{sgfs.Name}.m3u",
                    Path = $"{path}{sgfs.Name}.m3u",
                    IsDirectory = false,
                    Size = 4 * 1024 * 1024
                };
                yield return new DirectoryEntry
                {
                    Name = $"{sgfs.Name}.xml",
                    Path = $"{path}{sgfs.Name}.xml",
                    IsDirectory = false,
                    Size = 4 * 1024 * 1024
                };
            }
            else if (pathParts.Length == 2)
            {
                // ts or strm directory
                string directoryType = pathParts[1];
                if (directoryType is "ts" or "strm")
                {
                    foreach (SMFile smfs in sgfs.SMFS)
                    {
                        yield return new DirectoryEntry
                        {
                            Name = $"{smfs.Name}.{(directoryType == "ts" ? "ts" : "strm")}",
                            Path = $"{path}/{smfs.Name}.{(directoryType == "ts" ? "ts" : "strm")}",
                            IsDirectory = false,
                            Size = directoryType == "ts" ? long.MaxValue : 4 * 1024 * 1024
                        };
                    }
                }
                else
                {
                    yield return new DirectoryEntry
                    {
                        Name = $"{pathParts[1]}",
                        Path = $"{path}",
                        IsDirectory = false,
                        Size = 4 * 1024 * 1024
                    };
                }
            }
            else
            {
                string directoryType = pathParts[1];
                DirectoryEntry a = new()
                {
                    Name = $"{pathParts[2]}",
                    Path = $"{path}",
                    IsDirectory = false,
                    Size = directoryType == "ts" ? long.MaxValue : 4 * 1024 * 1024
                };

                yield return new DirectoryEntry
                {
                    Name = $"{pathParts[2]}",
                    Path = $"{path}",
                    IsDirectory = false,
                    Size = directoryType == "ts" ? long.MaxValue : 4 * 1024 * 1024
                };
            }
        }
    }

    private static string GetLastPathSegment(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        // Trim trailing and leading slashes
        path = path.Trim('/');

        // Split the path into segments and return the last one
        return path.Split('/').Last();
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken)
    {
        return Task.FromResult(path.EndsWith(".xml") || path.EndsWith(".m3u") || path.EndsWith(".strm"));
    }

    public Task SaveToLocalCacheAsync(string path, Stream content, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Virtual storage does not support caching.");
    }

    public Task CreateDirectoryAsync(string path, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Virtual storage does not support creating directories.");
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Virtual storage does not support deleting resources.");
    }

    public Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Virtual storage does not support copying resources.");
    }

    public Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Virtual storage does not support moving resources.");
    }

    private async Task<Stream> GenerateXmlStreamAsync(string path, CancellationToken cancellationToken)
    {
        string xmlContent = "<root><element>Example</element></root>";
        return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xmlContent));
    }

    private async Task<Stream> GenerateM3UStreamAsync(string path, CancellationToken cancellationToken)
    {
        string m3uContent = "#EXTM3U\n#EXTINF:-1,Example Stream\nhttp://example.com/stream.m3u8";
        return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(m3uContent));
    }

    private static MemoryStream GenerateStrmStream(string url)
    {
        return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(url));
    }
}