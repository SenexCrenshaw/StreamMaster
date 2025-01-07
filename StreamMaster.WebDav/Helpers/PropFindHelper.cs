using System.Globalization;

using StreamMaster.Domain.Configuration;
using StreamMaster.WebDav.Domain.Models;

namespace StreamMaster.WebDav.Helpers;

/// <summary>
/// Helper class for generating PROPFIND XML responses.
/// </summary>
public static class PropFindHelper
{
    /// <summary>
    /// Generates an XML response for a PROPFIND request.
    /// </summary>
    /// <param name="entries">Directory entries to include in the response.</param>
    /// <returns>An XML string representing the PROPFIND response.</returns>
    public static string GeneratePropFindResponse(IEnumerable<DirectoryEntry> entries, string basePath = "/webdav")
    {
        System.Text.StringBuilder xml = new();
        xml.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        xml.AppendLine("<D:multistatus xmlns:D='DAV:'>");

        foreach (DirectoryEntry entry in entries)
        {
            string thing = "";
            if (!entry.Path.StartsWith('/'))
            {
                thing = "/";
            }
            // Encode the path for proper URL handling
            string encodedPath = EncodeHref(basePath + thing + entry.Path, entry.IsDirectory);

            xml.AppendLine("<D:response>");
            xml.AppendLine($"<D:href>{encodedPath}</D:href>");
            xml.AppendLine("<D:propstat>");
            xml.AppendLine("<D:prop>");
            xml.AppendLine($"<D:displayname>{System.Security.SecurityElement.Escape(entry.Name)}</D:displayname>");

            if (entry.IsDirectory)
            {
                xml.AppendLine("<D:resourcetype><D:collection /></D:resourcetype>");
            }
            else
            {
                xml.AppendLine("<D:resourcetype />"); // Files have an empty <D:resourcetype />
            }

            // Add metadata for creation and modification dates
            //if (entry.CreationDate.HasValue)
            //{
            xml.AppendLine($"<D:creationdate>{BuildInfo.StartTime.ToString("o", CultureInfo.InvariantCulture)}</D:creationdate>");
            //}

            //if (entry.LastModified.HasValue)
            //{
            xml.AppendLine($"<D:getlastmodified>{BuildInfo.StartTime.ToString("R", CultureInfo.InvariantCulture)}</D:getlastmodified>");
            //}

            // Add size information for files and directories
            if (entry.IsDirectory)
            {
                xml.AppendLine("<D:getcontentlength>0</D:getcontentlength>");
            }
            else if (entry.Size.HasValue)
            {
                xml.AppendLine($"<D:getcontentlength>{entry.Size.Value}</D:getcontentlength>");
            }

            // Supported locks
            xml.AppendLine("<D:supportedlock>");
            xml.AppendLine("<D:lockentry>");
            xml.AppendLine("<D:lockscope><D:shared /></D:lockscope>");
            xml.AppendLine("<D:locktype><D:write /></D:locktype>");
            xml.AppendLine("</D:lockentry>");
            xml.AppendLine("<D:lockentry>");
            xml.AppendLine("<D:lockscope><D:exclusive /></D:lockscope>");
            xml.AppendLine("<D:locktype><D:write /></D:locktype>");
            xml.AppendLine("</D:lockentry>");
            xml.AppendLine("</D:supportedlock>");

            xml.AppendLine("</D:prop>");
            xml.AppendLine("<D:status>HTTP/1.1 200 OK</D:status>");
            xml.AppendLine("</D:propstat>");
            xml.AppendLine("</D:response>");
        }

        xml.AppendLine("</D:multistatus>");
        return xml.ToString();
    }

    private static string EncodeHref(string path, bool isDirectory)
    {
        if (string.IsNullOrEmpty(path))
        {
            return "/";
        }

        path = path.StartsWith("/") ? path : "/" + path;
        if (isDirectory && !path.EndsWith("/"))
        {
            path += "/";
        }
        return EscapeUriString(path);
    }

    private static string EscapeUriString(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return "/";
        }

        // Ensure the path ends with a slash if it's a directory
        path = path.EndsWith("/") ? path : path + "/";

        // Split the path into segments and encode each segment
        string[] segments = path.Split('/');
        IEnumerable<string> encodedSegments = segments.Select(Uri.EscapeDataString);

        // Reassemble the path with '/'
        return string.Join("/", encodedSegments);
    }
}