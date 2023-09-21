using StreamMaster.SchedulesDirectAPI;

namespace SDConsole;

internal class Program
{
    private static async Task Main(string[] args)
    {
        CancellationTokenSource cancellationTokenSource = new();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        SchedulesDirect sd = new("Mozilla/5.0 (compatible; streammaster/1.0)", "", "");

        Console.WriteLine("Getting Status");
        bool IsReady = await sd.GetSystemReady(cancellationToken);
        if (!IsReady)
        {
            Console.WriteLine("System offline");
            return;
        }

        Console.WriteLine("System Is Online");
        //await sd.GetImage("https://json.schedulesdirect.org/20141201/image/542065b533247587507ad1f9640c3ab951b41971ccb8dad4bc41c6eeae15bfdf.jpg", cancellationToken).ConfigureAwait(false);
        //await sd.GetImage("542065b533247587507ad1f9640c3ab951b41971ccb8dad4bc41c6eeae15bfdf.jpg", cancellationToken).ConfigureAwait(false);
        int a = 1;
        //Console.WriteLine("Getting Headends");
        //var headends = await sd.GetHeadends("", "", cancellationToken: cancellationToken);

        //Console.WriteLine("Getting Countries");
        //var countries = await sd.GetCountries(cancellationToken: cancellationToken);

        ////foreach (var l in status.lineups)
        ////{
        ////    Console.WriteLine(l);
        ////}

        //Console.WriteLine("Getting Lineups");
        //var lineups = await sd.GetLineups(cancellationToken: cancellationToken);

        //string fileName = "lineups.json";
        //string jsonString = JsonSerializer.Serialize(lineups);
        //File.WriteAllText(fileName, jsonString);

        //var lineup = await sd.GetLineup("", cancellationToken: cancellationToken);
        //if (lineup == null)
        //{
        //    Console.WriteLine($"lineup is null");
        //    return;
        //}

        //fileName = "lineup.json";
        //jsonString = JsonSerializer.Serialize(lineup);
        //File.WriteAllText(fileName, jsonString);

        ////lineup.OutputFormattedResult();

        //var testStation = lineup.Stations.FirstOrDefault(a => a.Callsign.ToLower().Contains(""));
        //if (testStation != null)
        //{
        //    fileName = "testStation.json";
        //    jsonString = JsonSerializer.Serialize(testStation);
        //    File.WriteAllText(fileName, jsonString);

        // Console.WriteLine("Getting Schedules"); var schedules = await
        // sd.GetSchedules(new List<string> { testStation.StationID },
        // cancellationToken: cancellationToken); if (schedules != null) {
        // fileName = "schedules.json"; jsonString =
        // JsonSerializer.Serialize(schedules); File.WriteAllText(fileName, jsonString);

        // var progIds = schedules.SelectMany(a => a.Programs).Select(a =>
        // a.ProgramID).Distinct().ToList(); Console.WriteLine("Getting
        // Programs"); var programs = await sd.GetPrograms(progIds,
        // cancellationToken: cancellationToken);

        //        fileName = "programs.json";
        //        jsonString = JsonSerializer.Serialize(programs);
        //        File.WriteAllText(fileName, jsonString);
        //    }
        //}

        cancellationTokenSource.Cancel();
    }
}
