using StreamMaster.PlayList.Models;

namespace StreamMaster.PlayList
{
    public interface IIntroPlayListBuilder
    {
        int IntroCount { get; }

        string GetIntroLogo(string introFileName);
        CustomPlayList? GetIntroPlayList(string Name);
        List<CustomPlayList> GetIntroPlayLists();
        CustomStreamNfo? GetRandomIntro(int? avoidIndex = null);
        string GetRandomSMStreamIntro();
    }
}