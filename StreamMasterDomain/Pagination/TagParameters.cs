namespace StreamMasterDomain.Pagination
{
    public class TagParameters : QueryStringParameters
    {
        public TagParameters()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
    }
}
