namespace StreamMasterDomain.Pagination
{
    public class EPGFileParameters : QueryStringParameters
    {
        public EPGFileParameters()
        {
            OrderBy = "name desc";
        }

    }
}
