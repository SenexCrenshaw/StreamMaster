using System.Diagnostics;
using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Helpers
{
    public static class ChannelHelper
    {
        public static Channel<byte[]> GetChannel(bool isCustomStream)
        {
            //return Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
            Debug.WriteLine($"ChannelHelper.GetChannel {isCustomStream}");
            return isCustomStream
                ? Channel.CreateBounded<byte[]>(new BoundedChannelOptions(200) { SingleReader = true, SingleWriter = true, FullMode = BoundedChannelFullMode.Wait }) :
              Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
        }
    }
}
