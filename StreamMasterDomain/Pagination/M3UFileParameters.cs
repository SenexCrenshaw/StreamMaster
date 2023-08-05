namespace StreamMasterInfrastructure.Pagination
{
    public class M3UFileParameters : QueryStringParameters
    {
        public M3UFileParameters()
        {
            OrderBy = "id desc";
        }

        public string Name { get; set; }
    }
}
