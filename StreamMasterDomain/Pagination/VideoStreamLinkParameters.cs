namespace StreamMasterDomain.Pagination;

public class VideoStreamLinkParameters : QueryStringParameters
{
    public VideoStreamLinkParameters()
    {
        OrderBy = "parentvideosteamid desc";
    }
}