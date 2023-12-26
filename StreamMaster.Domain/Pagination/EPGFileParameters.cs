namespace StreamMaster.Domain.Pagination
{
    public class EPGFileParameters : QueryStringParameters
    {
        public EPGFileParameters()
        {
            OrderBy = "name desc";
        }

    }
}
