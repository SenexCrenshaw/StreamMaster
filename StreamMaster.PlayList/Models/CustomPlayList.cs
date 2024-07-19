namespace StreamMaster.PlayList.Models;

public class CustomPlayList
{
    public string Name { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public Movie? FolderNfo { get; set; }
    public List<CustomStreamNfo> CustomStreamNfos { get; set; } = [];


}
