
namespace StreamMasterDomain.Pagination;

public class StreamGroupParameters : QueryStringParameters
{
    public StreamGroupParameters()
    {
        OrderBy = "name desc";
    }

    public string Name { get; set; }
}
