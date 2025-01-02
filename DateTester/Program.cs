using StreamMaster.Domain.Exceptions;
using StreamMaster.Domain.Helpers;
namespace DateTester;
public static class Program
{
    private static void Main()
    {
        IEnumerable<int> numbers = Enumerable.Range(1, 105); // Example input
        int batchSize = 50;

        foreach (IEnumerable<int> batch in numbers.Batch(batchSize))
        {
            Console.WriteLine($"Batch: {string.Join(", ", batch)}");
        }

        List<string> testDates = ["20240824240000 +0200", "202410270000", "20241032010000 +01:00", "20241102243000 +0200", "2024040830000 +0000"];
        string workingDate = "";
        try
        {
            foreach (string date in testDates)
            {
                workingDate = date;
                Console.WriteLine($"Checking {date}");
                DateTime test = ParseFlexibleDateHelper.ParseFlexibleDate(date);
                Console.WriteLine(test);
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"{workingDate} Error: {ex.Message}");
        }
    }
}
