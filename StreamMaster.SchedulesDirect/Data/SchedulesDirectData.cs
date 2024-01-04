using Microsoft.Extensions.Caching.Memory;

using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData(ILogger<SchedulesDirectData> logger, ILogger<EPGImportLogger> _epgImportLogger, IEPGHelper ePGHelper, IMemoryCache memoryCache, int EPGNumber) : ISchedulesDirectData
{
    public int EPGNumber { get; set; } = EPGNumber;

    [XmlArrayItem("Provider")]
    public ConcurrentBag<MxfProvider> Providers { get; set; } = [];

    [XmlAttribute("provider")]
    public string Provider { get; set; } = string.Empty;

    private static readonly object csvLock = new();

    public static readonly string serviceCSV = "services.csv";
    public static readonly string programsCSV = "programs.csv";

    public static bool WriteCSV { get; set; } = false;
    private void WriteToCSV(string fileName, string line)
    {
        if (!WriteCSV)
        {
            return;
        }
        lock (csvLock)
        {
            File.AppendAllText(fileName, line + "\r\n");
        }
    }
    public void ResetLists()
    {
        Affiliates.Clear();

        GuideImages.Clear();

        Keywords.Clear();

        KeywordGroups.Clear();

        Lineups.Clear();

        People.Clear();

        Programs.Clear();
        ProgramsToProcess.Clear();

        Providers.Clear();

        Seasons.Clear();
        SeasonsToProcess.Clear();

        SeriesInfos.Clear();
        SeriesInfosToProcess.Clear();
        ScheduleEntries.Clear();

        Services.Clear();
        ServicesToProcess.Clear();
    }

}
