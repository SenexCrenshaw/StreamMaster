namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData
{
    private Dictionary<string, MxfGuideImage> _guideImages = [];
    public MxfGuideImage FindOrCreateGuideImage(string pathname)
    {
        if (pathname.Contains("VC_CTI_USQA520417A_CBS_News_Mobile_210602_2022021"))
        {
            var a = 1;
        }
        if (_guideImages.TryGetValue(pathname, out MxfGuideImage? guideImage))
        {
            return guideImage;
        }

        GuideImages.Add(guideImage = new MxfGuideImage(GuideImages.Count + 1, pathname));
        _guideImages.Add(pathname, guideImage);
        return guideImage;
    }
}
