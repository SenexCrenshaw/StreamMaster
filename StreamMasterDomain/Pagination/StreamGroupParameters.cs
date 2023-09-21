
namespace StreamMasterDomain.Pagination;

public class StreamGroupParameters : QueryStringParameters
{
    public StreamGroupParameters()
    {
        OrderBy = "name desc";
    }

}
