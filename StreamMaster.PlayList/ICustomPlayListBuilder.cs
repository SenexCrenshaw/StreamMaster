using StreamMaster.PlayList.Models;
using StreamMaster.SchedulesDirect.Domain.XmltvXml;

namespace StreamMaster.PlayList;

public interface ICustomPlayListBuilder
{
    List<XmltvProgramme> GetXmltvProgrammeForPeriod(string customPlayListName, DateTime startDate, int days);
    List<(Movie Movie, DateTime StartTime, DateTime EndTime)> GetMoviesForPeriod(string customPlayListName, DateTime startDate, int days);
    CustomStreamNfo? GetIntro();
    (CustomStreamNfo StreamNfo, int SecondsIn) GetCurrentVideoAndElapsedSeconds(string customPlayListName);
    List<CustomPlayList> GetCustomPlayLists();
    CustomPlayList? GetCustomPlayList(string Name);
    Movie GetCustomPlayListByMovieId(string movieId);
}