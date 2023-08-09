namespace StreamMasterDomain.Pagination
{
    public class IconFileParameters : QueryStringParameters
    {
        public IconFileParameters()
        {
            OrderBy = "id name";
        }

        public string Name { get; set; }
    }
}