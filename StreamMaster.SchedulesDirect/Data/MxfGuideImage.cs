using StreamMaster.Domain.Extensions;

using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlArrayItem("GuideImage")]
    public ConcurrentDictionary<string, MxfGuideImage> GuideImages { get; set; } = [];

    public MxfGuideImage FindOrCreateGuideImage(string pathname)
    {
        return GuideImages.FindOrCreate(pathname, key => new MxfGuideImage(GuideImages.Count + 1, key));
    }
}
