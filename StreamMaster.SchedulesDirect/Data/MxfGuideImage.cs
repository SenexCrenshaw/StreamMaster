namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    private Dictionary<string, MxfGuideImage> _guideImages = [];
    public MxfGuideImage FindOrCreateGuideImage(string pathname)
    {
        if (_guideImages.TryGetValue(pathname, out MxfGuideImage? guideImage))
        {
            return guideImage;
        }
        guideImage = new MxfGuideImage(GuideImages.Count + 1, pathname);
        GuideImages.Add(guideImage);
        _guideImages.Add(pathname, guideImage);
        return guideImage;
    }
}
