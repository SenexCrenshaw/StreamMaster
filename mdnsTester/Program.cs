using StreamMaster.Mdns;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Starting mDNS Advertiser for two HDHomeRun devices...");

        using CancellationTokenSource cts = new();

        // Advertise two HDHR devices
        MdnsAdvertiser device1 = new("sm1", "_sm._tcp", 7095);
        //MdnsAdvertiser device2 = new("HDHomeRun2", "hdhomerun._tcp.local.", 7096);

        Task device1Task = device1.StartAsync(cts.Token);
        //Task device2Task = device2.StartAsync(cts.Token);

        Console.WriteLine("Press any key to stop...");
        Console.ReadKey();

        // Stop the advertisers
        await device1.StopAsync(cts.Token);
        //await device2.StopAsync(cts.Token);

        device1.Dispose();
        //device2.Dispose();

        Console.WriteLine("mDNS Advertiser stopped.");
    }
}
