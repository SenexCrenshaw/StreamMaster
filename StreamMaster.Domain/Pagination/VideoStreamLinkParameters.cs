namespace StreamMaster.Domain.Pagination;

public class VideoStreamLinkParameters : QueryStringParameters
{
    public VideoStreamLinkParameters()
    {
        OrderBy = "parentvideosteamid desc";
    }
}