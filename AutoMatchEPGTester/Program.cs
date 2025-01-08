using System.Collections.Concurrent;

using StreamMaster.Domain.XmltvXml;

namespace AutoMatchEPGTester
{
    internal record Test(string ChannelName, string ChannelEPGID, string ExpectedResult);

    internal class Program
    {
        private static readonly ConcurrentDictionary<int, List<StationChannelName>> StationChannelNames = [];

        private static void Main(string[] args)
        {
            StationChannelNames.TryAdd(-1,
                [
                    new("Showtime2.us", "[Showtime 2 US] Showtime 2 US","Showtime 2 US",  "logoEspn.png", 1),
                    new("10179", "[ESPN] ESPN","ESPN",  "logoEspn.png", -1),
                    new("59839",  "[HBO Comedy HD] HBOCHD","HBOCHD", "logoHboChd.png",  -1),
                    new("19548", "[HBO East] HBOHD", "HBOHD", "logoHboEast.png",  -1),
                    new("16585", "[HBO Family] HBOF", "HBOF", "logoHboFamily.png",  -1),
                    new("19612", "[WPVI-DT (American Broadcasting Company)]", "WPVIDT", "logoWPVI.png",  -1)
                ]);

            List<Test> tests =
            [
                new Test("Showtime 2 US", "-1-10179","19548"),
                new Test("Showtime US", "-1-10179","19548"),
                new Test("HBO", "-1-10179","19548"),
                new Test("HBO C", "-1-10179","59839"),
                new Test("HBO DT", "-1-10179","59839"),
                new Test("HBO Comedy", "-1-10179","59839"),
                new Test("HBO Fam", "-1-10179", "16585"),
                new Test("PA Philadelphia ABC 6 (WPVI)", "-1-10179","19612"),
            ];
            foreach (Test test in tests)
            {
                StationChannelName? result = EpgMatcher.MatchAsync(test.ChannelName, test.ChannelEPGID, StationChannelNames).Result;
                if (result?.Channel == test.ExpectedResult)
                {
                    Console.WriteLine("Test Passed");
                }
                else
                {
                    string c = result?.Channel ?? "";
                    Console.WriteLine($"Test Failed, expected {test.ExpectedResult} but got {c}");
                }
            }
        }
    }
}