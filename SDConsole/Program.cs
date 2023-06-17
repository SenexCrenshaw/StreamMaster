using StreamMaster.SchedulesDirect;

using StreamMasterDomain.Authentication;

using System.Text.Json;

namespace SDConsole;

internal class Program
{
    private static async Task Main(string[] args)
    {
        int originalValue = 123;
        string valueKey = "f835904d5a2343d8ac567c026d6c08b2";
        string serverKey = "ef1a2b3c4d5e6f7a8b9c0d1e2f3a4b5c";

        string encodedValue = originalValue.EncodeValue128("ef1a2b3c4d5e6f7a8b9c0d1e2f3a4b5c");

        Console.WriteLine($"Encoded value: {encodedValue}");

        // Decode the value
        int? decodedValue = encodedValue.DecodeValue128(serverKey);
        if (decodedValue.HasValue)
            Console.WriteLine($"Decoded value: {decodedValue}");
        else
            Console.WriteLine("Decoding failed.");

        return;

        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        Console.WriteLine("Getting Token");
        var sd = new SchedulesDirect("", "");

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
