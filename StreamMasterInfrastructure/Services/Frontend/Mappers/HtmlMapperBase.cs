using Microsoft.Extensions.Logging;

using System.Text.RegularExpressions;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public abstract class HtmlMapperBase : StaticResourceMapperBase
    {
        protected string HtmlPath;
        protected string UrlBase;
        private static readonly Regex ReplaceRegex = new Regex(@"(?:(?<attribute>href|src)=\"")(?<path>.*?(?<extension>css|js|png|ico|ics|svg|json))(?:\"")(?:\s(?<nohash>data-no-hash))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private string _generatedContent;

        protected HtmlMapperBase(ILogger logger) : base(logger)
        {
        }

        protected override Stream GetContentStream(string filePath)
        {
            HtmlPath = filePath;
            var text = GetHtmlText();

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(text);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        protected string GetHtmlText()
        {
            var text = File.ReadAllText(HtmlPath);

            text = ReplaceRegex.Replace(text, match =>
            {
                string url;

                url = match.Groups["path"].Value;

                return string.Format("{0}=\"{1}{2}\"", match.Groups["attribute"].Value, UrlBase, url);
            });

            _generatedContent = text;

            return _generatedContent;
        }
    }
}