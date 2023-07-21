using StreamMaster.SchedulesDirectAPI;

using StreamMasterDomain.Authentication;

using System.Text.Json;

namespace SDConsole;

internal class Program
{
    private static async Task Main(string[] args)
    {
        string serverKey = "a43710efe6df4bb18d0a65e3f93acbb1";

        int value1 = 123;
        int value2 = 456;
        string strValue = "Hello World";
        int StreamGroupNumber = 1;
        var teststr = "W6H3opcuhxZ-lOO7NZyp9NptIfRZFSoLlxeEw4ipDNmKogqOKcIklnzX2daYv9bc-F3TmDRAutP5N02_8VrTPA";

        var thing = StreamGroupNumber.EncodeValues128("00125dc876be8687dd57a1fd3405df44", serverKey);
        Console.WriteLine($"Encoded values: {thing}");
        var decodedSingleValues = thing.DecodeTwoValuesAsString128(serverKey);
        Console.WriteLine($"Decoded values: {decodedSingleValues.Item1} , {decodedSingleValues.Item2}");

        //var encodedStreamGroupNumber = StreamGroupNumber.EncodeValue128(serverKey);
        //Console.WriteLine($"Encoded single value: {encodedStreamGroupNumber}");
        //int? decodedSingleValue = encodedStreamGroupNumber.DecodeValue128(serverKey);
        //Console.WriteLine($"Decoded single value: {decodedSingleValue}");

        //int? testvalue = teststr.DecodeValue128(serverKey);
        //Console.WriteLine($"Decoded single value: {testvalue}");

        //var encodedStr = strValue.EncodeValue128(serverKey);
        //Console.WriteLine($"Encoded single value: {strValue} : {encodedStr}");
        //string? decodedStringSingleValue = encodedStr.DecodeValueAsString128(serverKey);
        //Console.WriteLine($"Decoded single value: {decodedStringSingleValue}");

        //string encodedSingleValue = value1.EncodeValue128(serverKey);
        //int? decodedSingleValue = encodedSingleValue.DecodeValue128(serverKey);

        //Console.WriteLine($"Encoded single value: {encodedSingleValue}");
        //Console.WriteLine($"Decoded single value: {decodedSingleValue}");

        //string encodedTwoValues = value1.EncodeValues128(value2, serverKey);
        //(int? decodedValue1, int? decodedValue2) = encodedTwoValues.DecodeValues128(serverKey);

        //Console.WriteLine($"Encoded two values: {encodedTwoValues}");
        //Console.WriteLine($"Decoded value 1: {decodedValue1}");
        //Console.WriteLine($"Decoded value 2: {decodedValue2}");

        return;

        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

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
        var headends = await sd.GetHeadends("", "", cancellationToken: cancellationToken);

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
