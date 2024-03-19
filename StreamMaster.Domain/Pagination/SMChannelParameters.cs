namespace StreamMaster.Domain.Pagination;

public class SMChannelParameters : QueryStringParameters
{
    public SMChannelParameters()
    {
        OrderBy = "name desc";
    }

}