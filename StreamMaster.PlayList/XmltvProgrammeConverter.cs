using System.Globalization;

using StreamMaster.Domain.XmltvXml;
using StreamMaster.PlayList.Models;

namespace StreamMaster.PlayList;

public static class XmltvProgrammeConverter
{
    public static XmltvProgramme ConvertMovieToXmltvProgramme(Movie movie, string channelId, DateTime StartTime, DateTime EndTime)
    {
        XmltvProgramme programme = new()
        {
            Titles = [new XmltvText { Text = movie.Title }],
            Descriptions = [new XmltvText { Text = movie.Plot }],
            Start = FormatDateTime(StartTime),
            Stop = FormatDateTime(EndTime),
            Channel = channelId,
            Categories = movie.Genres?.ConvertAll(g => new XmltvText { Text = g }),
            Countries = [new() { Text = movie.Country }],
            Rating = movie.Ratings?.Rating?.ConvertAll(r => new XmltvRating { Value = r.Value, System = r.Name }),
            StarRating = movie.Ratings?.Rating?.ConvertAll(r => new XmltvRating { Value = r.Value, System = r.Name }),
            Credits = new XmltvCredit
            {
                Actors = movie.Actors?.ConvertAll(a => new XmltvActor { Role = a.Role, Actor = a.Name })
            },
            EpisodeNums =
            [
                new XmltvEpisodeNum { System = "default", Text = movie.Id }
            ],
            Language = new XmltvText { Text = movie.Country }, // Assuming language is based on country
            Length = new XmltvLength { Units = "minutes", Text = movie.Runtime.ToString() },
            Video = new XmltvVideo
            {
                Aspect = movie.Fileinfo?.Streamdetails?.Video?.Aspect ?? "",
                Quality = movie.Rating // Assuming video quality based on rating
            }
        };

        return programme;
    }

    private static string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString("yyyyMMddHHmmss +0000", CultureInfo.InvariantCulture);
    }

}