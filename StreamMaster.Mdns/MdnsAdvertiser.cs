
using Makaretu.Dns;

namespace StreamMaster.Mdns
{
    public class MdnsAdvertiser : IAdvertiser
    {
        private readonly MulticastService multicastService;
        private readonly ServiceDiscovery serviceDiscovery;

        public MdnsAdvertiser(string serviceName, string serviceType, int port)
        {
            multicastService = new MulticastService();
            serviceDiscovery = new ServiceDiscovery(multicastService);

            ServiceProfile serviceProfile = new(serviceName, serviceType, (ushort)port);

            serviceProfile.AddProperty("deviceID", "12345678");
            serviceProfile.AddProperty("version", "1.0");

            serviceDiscovery.Advertise(serviceProfile);
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            multicastService.Start();
            Console.WriteLine("mDNS Advertiser started.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            multicastService.Stop();
            Console.WriteLine("mDNS Advertiser stopped.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            multicastService.Dispose();
        }
    }

}
