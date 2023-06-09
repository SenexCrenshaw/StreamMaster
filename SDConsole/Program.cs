using StreamMaster.SchedulesDirect;

using System.Text.Json;

namespace SDConsole;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        Console.WriteLine("Getting Token");
        var sd = new SchedulesDirect("senexcrenshaw", "IPTVR0xR0x");

        Console.WriteLine("Getting Status");
        var status = await sd.GetStatus(cancellationToken);
        if (status == null || !status.systemStatus.Any())
        {
            Console.WriteLine("Status is null");
            return;
        }

        var systemStatus = status.systemStatus[0];
        if (systemStatus.status == "Offline")
        {
            Console.WriteLine($"Status is {systemStatus.status}");
            return;
        }
        Console.WriteLine("System Is Online");

        Console.WriteLine("Getting Headends");
        var headends = await sd.GetHeadends(cancellationToken: cancellationToken);

        Console.WriteLine("Getting Countries");
        var countries = await sd.GetCountries(cancellationToken: cancellationToken);

        //foreach (var l in status.lineups)
        //{
        //    Console.WriteLine(l);
        //}

        Console.WriteLine("Getting Lineups");
        var lineups = await sd.GetLineups(cancellationToken: cancellationToken);

        string fileName = "lineups.json";
        string jsonString = JsonSerializer.Serialize(lineups);
        File.WriteAllText(fileName, jsonString);

        var lineup = await sd.GetLineup("USA-OTA-19087", cancellationToken: cancellationToken);
        if (lineup == null)
        {
            Console.WriteLine($"lineup is null");
            return;
        }

        fileName = "lineup.json";
        jsonString = JsonSerializer.Serialize(lineup);
        File.WriteAllText(fileName, jsonString);

        //lineup.OutputFormattedResult();

        var wpvi = lineup.Stations.FirstOrDefault(a => a.Callsign.ToLower().Contains("wpvi"));
        if (wpvi != null)
        {
            fileName = "wpvi.json";
            jsonString = JsonSerializer.Serialize(wpvi);
            File.WriteAllText(fileName, jsonString);

            Console.WriteLine("Getting Schedules");
            var schedules = await sd.GetSchedules(new List<string> { wpvi.StationID }, cancellationToken: cancellationToken);
            if (schedules != null)
            {
                fileName = "schedules.json";
                jsonString = JsonSerializer.Serialize(schedules);
                File.WriteAllText(fileName, jsonString);

                var progIds = schedules.SelectMany(a => a.Programs).Select(a => a.ProgramID).Distinct().ToList();
                Console.WriteLine("Getting Programs");
                var programs = await sd.GetPrograms(progIds, cancellationToken: cancellationToken);

                fileName = "programs.json";
                jsonString = JsonSerializer.Serialize(programs);
                File.WriteAllText(fileName, jsonString);
            }
        }

        cancellationTokenSource.Cancel();
    }
}
