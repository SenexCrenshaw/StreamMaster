﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Logging;
using StreamMaster.Domain.Services;
using StreamMaster.Infrastructure.Services.Settings;
using StreamMaster.SchedulesDirect;
using StreamMaster.SchedulesDirect.Converters;
using StreamMaster.SchedulesDirect.Data;
using StreamMaster.SchedulesDirect.Domain.Helpers;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.XmltvXml;
using StreamMaster.SchedulesDirect.Helpers;

using System.Xml;
using System.Xml.Serialization;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace EPGTester
{
    internal class Program
    {
        private static ILogger<Program> _logger;

        private static void Main(string[] args)
        {
            ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging(configure => configure.AddConsole())
            .AddSingleton<ISchedulesDirectDataService, SchedulesDirectDataService>()
            .AddTransient<IXmltv2Mxf, XmlTv2Mxf>()
            .AddSingleton<IMemoryCache, MemoryCache>()
            .AddSingleton<IEPGHelper, EPGHelper>()
            .AddSingleton<ISettingsService, SettingsService>()
            .AddSingleton<IXMLTVBuilder, XMLTVBuilder>()
            .BuildServiceProvider();

            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            GlobalLoggerProvider.Configure(loggerFactory);
            _logger = serviceProvider.GetService<ILogger<Program>>()!;

            IXmltv2Mxf? xmltv2Mxf = serviceProvider.GetService<IXmltv2Mxf>()!;

            string fullName = "C:\\Users\\senex\\git\\test\\epg123.xml";
            if (File.Exists(fullName) == false)
            {
                _logger.LogInformation($"File {fullName} does not exist");
                return;
            }

            XMLTV? epgData = xmltv2Mxf.ConvertToMxf(fullName, 0);
            if (epgData == null)
            {
                _logger.LogCritical("Exception EPG {fullName} format is not supported", fullName);
                return;
            }
            string channel = "ABCWAAY.us";

            List<XmltvProgramme> a = epgData.Programs.Where(a => a.Channel == channel).ToList();

            _logger.LogInformation("EPG {channel} has {a.Count} programs", channel, a.Count);

            string name = Path.GetFileNameWithoutExtension(fullName);
            string text = SerializeXMLTVData(epgData);
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
