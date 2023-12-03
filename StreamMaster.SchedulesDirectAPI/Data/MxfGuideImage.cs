namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData
{
    private readonly Dictionary<string, MxfGuideImage> _guideImages = [];
    public MxfGuideImage FindOrCreateGuideImage(string pathname)
    {
        if (_guideImages.TryGetValue(pathname, out MxfGuideImage? guideImage))
        {
            return guideImage;
        }

        GuideImages.Add(guideImage = new MxfGuideImage(GuideImages.Count + 1, pathname));
        _guideImages.Add(pathname, guideImage);
        return guideImage;
    }
}
