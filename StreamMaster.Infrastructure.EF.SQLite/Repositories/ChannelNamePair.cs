namespace StreamMaster.Infrastructure.EF.Repositories;

internal class ChannelNamePair
{
    public string Channel { get; set; }
    public string Name { get; set; }

    // You may want to override Equals and GetHashCode to ensure distinct works correctly.
    public override bool Equals(object obj)
    {
        return obj is ChannelNamePair other && Channel == other.Channel && Name == other.Name;
    }

    public override int GetHashCode()
    {
        // This is a simple way to get a combined hash code, there are other methods too.
        int hashChannel = Channel == null ? 0 : Channel.GetHashCode();
        int hashName = Name == null ? 0 : Name.GetHashCode();

        return hashChannel ^ hashName;
    }
}

