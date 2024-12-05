using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.XmltvXml;
using StreamMaster.Streams.Handlers;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace StreamMaster.API.Controllers;

public class MiscController(IImageDownloadService imageDownloadService, ICacheManager cacheManager, ILogoService logoService, ISourceBroadcasterService channelDistributorService) : ApiControllerBase
{
    [HttpGet]
    [Route("[action]")]
    public ActionResult<ImageDownloadServiceStatus> GetDownloadServiceStatus()
    {
        return Ok(imageDownloadService.ImageDownloadServiceStatus);
    }

    [HttpGet]
    [Route("[action]")]
    public ActionResult<List<SourceBroadcaster>> GetChannelDiGetChannelDistributors()
    {
        List<ISourceBroadcaster> channelDistributors = channelDistributorService.GetStreamBroadcasters();
        return Ok(channelDistributors);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> CacheSIcons(CancellationToken cancellationToken)
    {
        await logoService.CacheSMChannelLogosAsync(cancellationToken);
        await logoService.AddSMStreamLogosAsync(cancellationToken);

        return Ok();
    }

    [HttpGet]
    [Route("[action]")]
    public ActionResult<IDictionary<string, IStreamHandlerMetrics>> GetAggregatedMetrics()
    {
        IDictionary<string, IStreamHandlerMetrics> metrics = channelDistributorService.GetMetrics();
        return Ok(metrics);
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult GetTestM3U(int numberOfStreams)
    {
        List<string> lines = [];
        lines.Add("#EXTM3U");
        for (int i = 0; i < numberOfStreams; i++)
        {
            string id = Guid.NewGuid().ToString();
            lines.Add($"#EXTINF:-1 tvg-id=\"Channel_{i}\" tvg-name=\"Channel {i}\" tvg-chno=\"{i}\" tvg-logo=\"https://logo{i}.png\" group-title=\"TEST CHANNEL GROUP\", Channel {i}");
            lines.Add($"http://channelfake.test/live/{id}.ts");
        }

        string data = string.Join("\r\n", lines);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-test-{numberOfStreams}.m3u"
        };
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult GetTestEPG(int NumberOfChannels, int NumberOfDays)
    {
        XMLTV xmltv = new()
        {
            Date = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
            SourceInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
            SourceInfoName = "Stream Master",
            GeneratorInfoName = "Stream Master",
            GeneratorInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
            Channels = [],
            Programs = []
        };

        for (int i = 0; i < NumberOfChannels; i++)
        {
            StationChannelName? sn = cacheManager.GetStationChannelNames.GetRandomEntry();

            xmltv.Channels.Add(new XmltvChannel
            {
                Id = $"Channel_{i}",
                DisplayNames = [new XmltvText { Language = "en", Text = $"Channel_{i}" }],
                Icons = [new XmltvIcon { Src = sn?.Logo ?? "" }],
            });
        }

        for (int d = -2; d < NumberOfDays; d++)
        {
            // Start at midnight (00:00:00)
            string Start = new DateTimeOffset(DateTime.UtcNow.AddDays(d).Date).ToString("yyyyMMddHHmmss zzz", CultureInfo.InvariantCulture);

            // Stop at the end of the day (23:59:59)
            string Stop = new DateTimeOffset(DateTime.UtcNow.AddDays(d).Date.AddDays(1).AddSeconds(-1)).ToString("yyyyMMddHHmmss zzz", CultureInfo.InvariantCulture);

            for (int i = 0; i < NumberOfChannels; i++)
            {
                StationChannelName? sn = cacheManager.GetStationChannelNames.GetRandomEntry();
                xmltv.Programs.Add(new XmltvProgramme
                {
                    Start = Start,
                    Stop = Stop,
                    Channel = $"Channel_{i}",
                    Titles = [new XmltvText { Language = "en", Text = $"Programme_{i} {Start}" }],
                    Descriptions = [new XmltvText { Language = "en", Text = $"Description_{i} {Start}" }],
                    Icons = [new XmltvIcon { Src = sn?.Logo ?? "" }],
                });
            }
        }
        string xml = SerializeXMLTVData(xmltv);
        return new FileContentResult(Encoding.UTF8.GetBytes(xml), "application/xml")
        {
            FileDownloadName = $"epg-{NumberOfChannels}-{NumberOfDays}.xml"
        };
    }

    private static string SerializeXMLTVData(XMLTV xmltv)
    {
        XmlSerializerNamespaces ns = new();
        ns.Add("", "");

        // Create a Utf8StringWriter
        using Utf8StringWriter textWriter = new();

        XmlWriterSettings xmlSettings = new()
        {
            Indent = true,
            //OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.None,
            //NewLineChars = "\n"
        };

        // Create an XmlWriter using Utf8StringWriter
        using XmlWriter writer = XmlWriter.Create(textWriter, xmlSettings);

        XmlSerializer xml = new(typeof(XMLTV));

        // Serialize XML data to the Utf8StringWriter
        xml.Serialize(writer, xmltv, ns);

        // Get the XML string from the Utf8StringWriter
        string xmlText = textWriter.ToString();

        return xmlText;
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> Backup()
    {
        await FileUtil.Backup();
        return Ok();
    }
}