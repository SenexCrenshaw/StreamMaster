using Microsoft.Extensions.Hosting;

namespace StreamMaster.Mdns
{
    public class MdnsService : IHostedService, IDisposable
    {
        private readonly IAdvertiser _advertiser;

        public MdnsService()
        {
            // Initialize MdnsAdvertiser with appropriate parameters
            _advertiser = new MdnsAdvertiser("HDHomeRun1", "_hdhomerun._tcp.local.", 7095);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _advertiser.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _advertiser.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            _advertiser.Dispose();
        }
    }
}
