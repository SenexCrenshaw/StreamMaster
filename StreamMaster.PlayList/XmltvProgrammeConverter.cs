using StreamMaster.PlayList.Models;
using StreamMaster.SchedulesDirect.Domain.XmltvXml;

using System.Globalization;

namespace StreamMaster.PlayList;

public static class XmltvProgrammeConverter
{
    public static XmltvProgramme ConvertMovieToXmltvProgramme(Movie movie)
    {
        XmltvProgramme programme = new()
        {
            Titles = [new XmltvText { Text = movie.Title }],
            Descriptions = [new XmltvText { Text = movie.Plot }],
            Start = FormatDateTime(movie.Premiered),
            Stop = FormatDateTime(movie.Premiered, movie.Runtime),
            Channel = movie.Studio,
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
                Aspect = movie.Fileinfo?.Streamdetails?.Video?.Aspect,
                Quality = movie.Rating // Assuming video quality based on rating
            }
        };

        return programme;
    }

    private static string FormatDateTime(string premiered, int? runtime = null)
    {
        if (DateTime.TryParse(premiered, out DateTime premiereDate))
        {
            if (runtime.HasValue)
            {
                premiereDate = premiereDate.AddMinutes(runtime.Value);
            }
            return premiereDate.ToString("yyyyMMddHHmmss K", CultureInfo.InvariantCulture);
        }
        return string.Empty;
    }
}