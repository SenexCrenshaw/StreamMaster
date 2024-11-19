using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData(int EPGNumber) : ISchedulesDirectData
{
    public int EPGNumber { get; set; } = EPGNumber;

    [XmlArrayItem("Provider")]
    public ConcurrentBag<MxfProvider> Providers { get; set; } = [];

    [XmlAttribute("provider")]
    public string Provider { get; set; } = string.Empty;

    private readonly SemaphoreSlim csvSemaphore = new(1, 1);

    public readonly string serviceCSV = "services_" + EPGNumber + ".csv";
    public readonly string programsCSV = "programs_" + EPGNumber + ".csv";

    public static bool WriteCSV { get; set; } = false;
    private async Task WriteToCSVAsync(string fileName, string line)
    {
        if (!WriteCSV)
        {
            return;
        }

        await csvSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            // Asynchronous write to minimize contention.
            await File.AppendAllTextAsync(fileName, line + "\r\n").ConfigureAwait(false);
        }
        finally
        {
            csvSemaphore.Release();
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
        //ScheduleEntries.Clear();

        Services.Clear();
        //ServicesToProcess.Clear();
    }
}
