namespace StreamMaster.Domain.Pagination;

public class SMStreamParameters : QueryStringParameters
{
    public SMStreamParameters()
    {
        OrderBy = "name desc";
    }

}