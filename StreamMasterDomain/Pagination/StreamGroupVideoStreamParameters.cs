namespace StreamMasterDomain.Pagination;

public class StreamGroupVideoStreamParameters : QueryStringParameters
{
    public StreamGroupVideoStreamParameters()
    {
        OrderBy = "name desc";
    }

    public string Name { get; set; }
}