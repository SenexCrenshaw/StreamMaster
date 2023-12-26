namespace StreamMaster.Domain.Pagination
{
    public class ChannelGroupParameters : QueryStringParameters
    {
        public ChannelGroupParameters()
        {
            OrderBy = "name";
        }

    }
}
