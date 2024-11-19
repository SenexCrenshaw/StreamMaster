using Reinforced.Typings.Attributes;

using StreamMaster.PlayList.Models;
namespace StreamMaster.PlayList;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class CustomStreamNfo
{
    public CustomStreamNfo() { }
    public CustomStreamNfo(string VideoFileName, Movie Movie)
    {
        this.VideoFileName = VideoFileName;
        this.Movie = Movie;

    }
    public string VideoFileName { get; set; } = string.Empty;
    public Movie Movie { get; set; } = new();
}
