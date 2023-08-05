namespace StreamMasterDomain.Pagination
{
    public class ChannelGroupParameters : QueryStringParameters
    {
        public ChannelGroupParameters()
        {
            OrderBy = "name desc";
        }

        public string Name { get; set; }
    }
}
