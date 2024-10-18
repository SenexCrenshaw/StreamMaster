namespace StreamMaster.Streams.Domain.Interfaces;

public interface IIntroStatus
{
    int IntroIndex { get; set; }
    bool PlayedIntro { get; set; }
    bool IsFirst { get; set; }
}
