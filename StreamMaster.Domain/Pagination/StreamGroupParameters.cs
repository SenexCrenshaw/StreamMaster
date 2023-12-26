namespace StreamMaster.Domain.Pagination;

public class StreamGroupParameters : QueryStringParameters
{
    public StreamGroupParameters()
    {
        OrderBy = "name desc";
    }

}
