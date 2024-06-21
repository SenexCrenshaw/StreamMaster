using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Models;

public class StreamInfo
{
    public int Rank { get; set; }
    public SMStream SMStream { get; set; }
    public SMChannel SMChannel { get; set; }
}
