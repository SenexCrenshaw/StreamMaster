namespace StreamMasterDomain.Pagination
{
    public class ChannelGroupParameters : QueryStringParameters
    {
        public ChannelGroupParameters()
        {
            OrderBy = "name";
        }

        public string Name { get; set; }
    }
}
