namespace StreamMasterDomain.Pagination;

public class VideoStreamLinkParameters : QueryStringParameters
{
    public VideoStreamLinkParameters()
    {
        OrderBy = "user_tvg_name desc";
    }
}