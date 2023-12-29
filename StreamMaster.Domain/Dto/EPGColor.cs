using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Dto;

public class EPGColorDto : IMapFrom<EPGFile>
{
    public int Id { get; set; }
    //public int EPGFileId { get; set; }
    //public string CallSign { get; set; }
    public string StationId { get; set; }
    public string Color { get; set; }
}
