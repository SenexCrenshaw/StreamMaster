
namespace StreamMasterDomain.Pagination;

public class VideoStreamParameters : QueryStringParameters
{
    public VideoStreamParameters()
    {
        OrderBy = "user_tvg_name desc";
    }

    public string Name { get; set; }

    public static explicit operator VideoStreamParameters(ChannelGroupParameters v)
    {
        throw new NotImplementedException();
    }
}
