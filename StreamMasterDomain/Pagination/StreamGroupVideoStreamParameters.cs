namespace StreamMasterDomain.Pagination;

public class StreamGroupVideoStreamParameters : QueryStringParameters
{
    public StreamGroupVideoStreamParameters()
    {
        OrderBy = "streamgroupid desc";
    }
    public int StreamGroupId { get; set; }
}