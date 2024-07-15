using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

public class SMChannelStreamLink
{
    public int SMChannelId { get; set; }
    public SMChannel SMChannel { get; set; }
    [Column(TypeName = "citext")]
    public string SMStreamId { get; set; }
    public SMStream SMStream { get; set; }
    public int Rank { get; set; }
}