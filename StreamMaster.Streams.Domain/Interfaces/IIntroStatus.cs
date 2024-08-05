namespace StreamMaster.Streams.Domain.Interfaces;

public interface IIntroStatus
{
    int IntroIndex { get; set; }
    bool PlayedIntro { get; set; }
    bool IsFirst { get; set; }
}

//public abstract class IntroStatus : IIntroStatus
//{
//    public int IntroIndex { get; set; }
//    public bool PlayedIntro { get; set; }
//    public bool IsFirst { get; set; } = true;
//}
