using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Logging;
using StreamMaster.Domain.Services;
using StreamMaster.Infrastructure.Services.Settings;
using StreamMaster.SchedulesDirect;
using StreamMaster.SchedulesDirect.Converters;
using StreamMaster.SchedulesDirect.Data;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.XmltvXml;

using System.Xml;
using System.Xml.Serialization;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace M3UFormatter
{
    internal class Program
    {
        private static ILogger<Program> _logger;
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
            .AddLogging(configure => configure.AddConsole())
            .AddSingleton<ISchedulesDirectDataService, SchedulesDirectDataService>()
            .AddSingleton<IXmltv2Mxf, XmlTv2Mxf>()
            .AddSingleton<IMemoryCache, MemoryCache>()
            .AddSingleton<ISettingsService, SettingsService>()
            .AddSingleton<IXMLTVBuilder, XMLTVBuilder>()
            .BuildServiceProvider();

            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            GlobalLoggerProvider.Configure(loggerFactory);

            // Resolve the service
            var xmltv2Mxf = serviceProvider.GetService<IXmltv2Mxf>();

            var logger = serviceProvider.GetService<ILogger<Program>>();


            if (args.Length != 1 || string.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine("Usage: M3UFormatter <m3u file>");
                return;
            }

            var fullName = args[0];
            if (File.Exists(fullName) == false)
            {
                Console.WriteLine($"File {fullName} does not exist");
                return;
            }

            XMLTV? epgData = xmltv2Mxf.ConvertToMxf(fullName, 0);
            if (epgData == null)
            {
                logger.LogCritical("Exception EPG {fullName} format is not supported", fullName);
                return;
            }
            var name = Path.GetFileNameWithoutExtension(fullName);
            var text = SerializeXMLTVData(epgData);
            File.WriteAllText($"C:\\Users\\senex\\git\\test\\{name}_clean.xml", text);
        }

        private static string SerializeXMLTVData(XMLTV xmltv)
        {
            XmlSerializerNamespaces ns = new();
            ns.Add("", "");

            // Create a Utf8StringWriter
            using Utf8StringWriter textWriter = new();

            XmlWriterSettings settings = new()
            {
                Indent = true,
                OmitXmlDeclaration = true,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineChars = "\n"
            };

            // Create an XmlWriter using Utf8StringWriter
            using XmlWriter writer = XmlWriter.Create(textWriter, settings);

            XmlSerializer xml = new(typeof(XMLTV));

            // Serialize XML data to the Utf8StringWriter
            xml.Serialize(writer, xmltv, ns);

            // Get the XML string from the Utf8StringWriter
            string xmlText = textWriter.ToString();

            return xmlText;
        }
    }
}
