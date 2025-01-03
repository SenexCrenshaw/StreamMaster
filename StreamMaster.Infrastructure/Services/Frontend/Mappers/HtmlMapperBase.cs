using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public abstract partial class HtmlMapperBase(ILogger logger) : StaticResourceMapperBase(logger)
    {
        protected string HtmlPath = string.Empty;
        protected string UrlBase = string.Empty;
        private static readonly Regex ReplaceRegex = MyRegex();

        protected override async Task<Stream> GetContentStreamAsync(string filePath)
        {
            HtmlPath = filePath;
            return await GetHtmlTextAsync(filePath).ConfigureAwait(false);
        }

        protected async Task<Stream> GetHtmlTextAsync(string filePath)
        {
            HtmlPath = filePath;

            // Open the file as a FileStream asynchronously
            FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

            // Create a memory stream to apply the regex modifications
            MemoryStream modifiedStream = new();
            using (StreamReader reader = new(fileStream))
            {
                await using StreamWriter writer = new(modifiedStream, leaveOpen: true);
                string text = await reader.ReadToEndAsync().ConfigureAwait(false);
                text = ReplaceRegex.Replace(text, match =>
                {
                    string url = match.Groups["path"].Value;
                    return string.Format("{0}=\"{1}{2}\"", match.Groups["attribute"].Value, UrlBase, url);
                });

                await writer.WriteAsync(text).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }

            modifiedStream.Position = 0; // Reset position for reading from the beginning
            return modifiedStream;
        }

        [GeneratedRegex(@"(?:(?<attribute>href|src)=\"")(?<path>.*?(?<extension>css|js|png|ico|ics|svg|json))(?:\"")(?:\s(?<nohash>data-no-hash))?", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex MyRegex();
    }
}