namespace StreamMaster.PlayList
{
    public interface ICustomPlayListBuilder
    {
        List<CustomPlayList> GetCustomPlayLists();
        CustomPlayList? GetCustomPlayList(string Name);
    }
}