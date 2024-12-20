using System.Globalization;

using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.XML;
using StreamMaster.Streams.Domain.Interfaces;

namespace StreamMaster.SchedulesDirect.Converters;

public class SDXMLTVBuilder(
    IXmltvProgramBuilder xmltvProgramBuilder,
    IXmltvChannelBuilder xmltvChannelBuilder,
    IDataPreparationService dataPreparationService,
    IFileUtilService fileUtilService,
    ICacheManager cacheManager,
    ILogger<SDXMLTVBuilder> logger) : ISDXMLTVBuilder
{
    public XMLTV? CreateSDXmlTv()
    {
        try
        {
            //_dataPreparationService.Initialize(baseUrl, null);
            cacheManager.ClearEPGDataByEPGNumber(EPGHelper.SchedulesDirectId);

            XMLTV xmlTv = ProcessSDServices();

            xmlTv.SortXmlTv();

            _ = fileUtilService.ProcessStationChannelNamesAsync(BuildInfo.SDXMLFile, EPGHelper.SchedulesDirectId);
            return xmlTv;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to create the XMLTV file. Exception: {Message}", ex.Message);
            return null;
        }
    }

    private static XMLTV InitializeXmlTv()
    {
        return new XMLTV
        {
            Date = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
            SourceInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
            SourceInfoName = "Stream Master",
            GeneratorInfoName = "Stream Master",
            GeneratorInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
            Channels = [],
            Programs = []
        };
    }

    private XMLTV ProcessSDServices()
    {
        List<MxfService> services = dataPreparationService.GetAllSdServices();

        XMLTV xmlTv = InitializeXmlTv();

        //string baseUrl = GetUrlWithPath();

        _ = Parallel.ForEach(services, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, service =>
        {
            XmltvChannel channel = xmltvChannelBuilder.BuildXmltvChannel(service, true);
            List<MxfScheduleEntry> scheduleEntries = service.MxfScheduleEntries.ScheduleEntry;

            lock (xmlTv.Channels)
            {
                xmlTv.Channels.Add(channel);
            }

            dataPreparationService.AdjustServiceSchedules(service);

            List<XmltvProgramme> xmltvProgrammes = scheduleEntries.AsParallel().Select(scheduleEntry =>
                    xmltvProgramBuilder.BuildXmltvProgram(scheduleEntry, channel.Id, 0, "")).ToList();

            lock (xmlTv.Programs)
            {
                xmlTv.Programs.AddRange(xmltvProgrammes);
            }
        });

        return xmlTv;
    }
}