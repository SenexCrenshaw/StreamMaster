using System.Globalization;
using System.Text;

using Microsoft.Extensions.Logging;

using StreamMaster.Domain.XmltvXml;

namespace StreamMaster.Infrastructure.Services;

/// <summary>
/// Provides functionality to parse XMLTV channels from an EPG file.
/// </summary>
public interface IXmltvParser
{
    /// <summary>
    /// Asynchronously parses channels from the given EPG file path until a programme element is encountered.
    /// </summary>
    /// <param name="epgPath">The path to the EPG file.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of parsed channels.</returns>
    Task<List<XmltvChannel>?> GetChannelsFromXmlAsync(Stream stream, CancellationToken cancellationToken);
}

/// <summary>
/// An implementation of <see cref="IXmltvParser"/> that uses a custom, line-based parsing approach to quickly
/// extract channel information until the first &lt;programme&gt; tag is encountered.
/// </summary>
public sealed class XmltvParser(ILogger<XmltvParser> logger) : IXmltvParser
{
    /// <inheritdoc/>
    public async Task<List<XmltvChannel>?> GetChannelsFromXmlAsync(Stream fileStream, CancellationToken cancellationToken)
    {
        try
        {
            if (fileStream is null)
            {
                return null;
            }

            List<XmltvChannel> channels = [];
            using StreamReader reader = new(fileStream, Encoding.UTF8, true, 1024, false);

            // Parsing state
            bool inChannel = false;
            string currentChannelId = string.Empty;
            List<XmltvText> currentDisplayNames = [];
            List<XmltvIcon> currentIcons = [];

            while (!reader.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string? line = await reader.ReadLineAsync().ConfigureAwait(false);
                if (line is null)
                {
                    break;
                }

                // Once we hit <programme>, we stop parsing channels
                if (line.Contains("<programme", StringComparison.OrdinalIgnoreCase))
                {
                    // If we were currently parsing a channel, finalize it
                    if (inChannel)
                    {
                        channels.Add(new XmltvChannel(
                            currentChannelId,
                            currentDisplayNames,
                            currentIcons));
                    }
                    break;
                }

                // Detect channel start
                if (line.Contains("<channel", StringComparison.OrdinalIgnoreCase))
                {
                    // If we already had a channel open, close it before starting a new one
                    if (inChannel)
                    {
                        channels.Add(new XmltvChannel(
                            currentChannelId,
                            currentDisplayNames,
                            currentIcons));
                    }

                    inChannel = true;
                    currentDisplayNames.Clear();
                    currentIcons.Clear();
                    currentChannelId = ParseAttribute(line, "id") ?? string.Empty;
                    continue;
                }

                // Detect channel end
                if (inChannel && line.Contains("</channel", StringComparison.OrdinalIgnoreCase))
                {
                    channels.Add(new XmltvChannel(
                        currentChannelId,
                        currentDisplayNames,
                        currentIcons));
                    inChannel = false;
                    continue;
                }

                if (!inChannel)
                {
                    continue;
                }

                // Parse display-name
                if (line.Contains("<display-name>", StringComparison.OrdinalIgnoreCase))
                {
                    string nameValue = ParseElementValue(line, "display-name") ?? string.Empty;
                    currentDisplayNames.Add(new XmltvText(nameValue));
                    continue;
                }

                // Parse icon
                if (line.Contains("<icon", StringComparison.OrdinalIgnoreCase))
                {
                    string src = ParseAttribute(line, "src") ?? string.Empty;
                    int width = ParseIntAttribute(line, "width");
                    int height = ParseIntAttribute(line, "height");
                    currentIcons.Add(new XmltvIcon(src, width, height));
                    continue;
                }
            }
            return channels;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Channel parsing cancelled for file");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing channels from file");
        }

        return null;
    }

    /// <summary>
    /// Parses an attribute value from a line of XML-like text.
    /// </summary>
    /// <param name="line">The line containing the attribute.</param>
    /// <param name="attributeName">The attribute name to search for.</param>
    /// <returns>The attribute value or null if not found.</returns>
    private static string? ParseAttribute(string line, string attributeName)
    {
        // e.g. attributeName="value"
        int startIndex = line.IndexOf(attributeName + "=", StringComparison.OrdinalIgnoreCase);
        if (startIndex == -1)
        {
            return null;
        }

        // Move after 'attributeName="'
        startIndex = line.IndexOf('"', startIndex);
        if (startIndex == -1)
        {
            return null;
        }
        startIndex++;

        int endIndex = line.IndexOf('"', startIndex);
        return endIndex == -1 ? null : line[startIndex..endIndex].Trim();
    }

    /// <summary>
    /// Parses an integer attribute value from a line of XML-like text.
    /// Returns 0 if parsing fails.
    /// </summary>
    /// <param name="line">The line containing the attribute.</param>
    /// <param name="attributeName">The attribute name to search for.</param>
    /// <returns>The integer value, or 0 if not found or invalid.</returns>
    private static int ParseIntAttribute(string line, string attributeName)
    {
        string? val = ParseAttribute(line, attributeName);
        return val is null ? 0 : int.TryParse(val, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : 0;
    }

    /// <summary>
    /// Parses an element's inner text from a line of XML-like text.
    /// For example, &lt;display-name&gt;Channel 1&lt;/display-name&gt; returns "Channel 1".
    /// </summary>
    /// <param name="line">The line to parse.</param>
    /// <param name="elementName">The element name to find.</param>
    /// <returns>The inner text or null if not found.</returns>
    private static string? ParseElementValue(string line, string elementName)
    {
        string startTag = "<" + elementName + ">";
        string endTag = "</" + elementName + ">";

        int startIndex = line.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
        if (startIndex == -1)
        {
            return null;
        }

        startIndex += startTag.Length;
        int endIndex = line.IndexOf(endTag, startIndex, StringComparison.OrdinalIgnoreCase);
        return endIndex == -1 ? null : line[startIndex..endIndex].Trim();
    }
}