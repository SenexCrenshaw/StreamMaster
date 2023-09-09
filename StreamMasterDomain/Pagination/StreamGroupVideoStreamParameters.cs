namespace StreamMasterDomain.Pagination;

public class StreamGroupVideoStreamParameters : QueryStringParameters
{
    public StreamGroupVideoStreamParameters()
    {
        OrderBy = "name desc";
    }
}