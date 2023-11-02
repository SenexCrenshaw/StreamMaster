using StreamMaster.SchedulesDirectAPI.Domain.EPG;

using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace StreamMaster.SchedulesDirectAPI.Helpers;

public static class SDHelpers
{
    public static string GenerateHashFromStringContent(StringContent content)
    {
        byte[] contentBytes = Encoding.UTF8.GetBytes(content.ReadAsStringAsync().Result); // Extract string from StringContent and convert to bytes
        byte[] hashBytes = SHA256.HashData(contentBytes); // Compute SHA-256 hash
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Convert byte array to hex string
    }

    public static HttpClient CreateHttpClient(string clientUserAgent)
    {
        HttpClient client = new(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
        });
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd(clientUserAgent);

        return client;
    }
    public static string GenerateCacheKey(string command)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        StringBuilder sanitized = new(command.Length);
        foreach (char c in command)
        {
            if (!invalidChars.Contains(c))
            {
                sanitized.Append(c);
            }
            else
            {
                sanitized.Append('_');  // replace invalid chars with underscore or another desired character
            }
        }
        sanitized.Append(".json");
        return sanitized.ToString();
    }

    public static List<TvTitle> GetTitles(List<Title> Titles, string lang)
    {

        List<TvTitle> ret = Titles.ConvertAll(a => new TvTitle
        {
            Lang = lang,
            Text = a.Title120
        });
        return ret;
    }

    public static TvSubtitle GetSubTitles(ISDProgram sdProgram, string lang)
    {
        if (!string.IsNullOrEmpty(sdProgram.EpisodeTitle150))
        {
            return new TvSubtitle
            {
                Lang = lang,
                Text = sdProgram.EpisodeTitle150
            };
        }

        string description = "";
        if (sdProgram.Descriptions?.Description100 is not null)
        {
            IDescription100? test = sdProgram.Descriptions.Description100.FirstOrDefault(a => a.DescriptionLanguage == lang && a.Description != "");
            if (test == null)
            {
                if (!string.IsNullOrEmpty(sdProgram.Descriptions.Description100[0].Description))
                {
                    description = sdProgram.Descriptions.Description100[0].Description;
                }
            }
            else
            {
                description = test.Description;
            }
        }

        return new TvSubtitle
        {
            Lang = lang,
            Text = description
        };
    }

    public static TvCredits GetCredits(ISDProgram sdProgram, string lang)
    {
        TvCredits ret = new();
        if (sdProgram.Crew == null)
        {
            return ret;

        }
        foreach (Crew crew in sdProgram.Crew)
        {
            switch (crew.Role)
            {
                case "Actor":
                    ret.Actor ??= new();
                    ret.Actor.Add(new TvActor
                    {
                        Text = crew.Name,
                        Role = crew.Role
                    });
                    break;
                case "Director":
                    ret.Director ??= new();
                    ret.Director.Add(crew.Name);
                    break;
                case "Producer":
                    ret.Producer ??= new();
                    ret.Producer.Add(crew.Name);
                    break;
                case "Presenter":
                    ret.Presenter ??= new();
                    ret.Presenter.Add(crew.Name);
                    break;
                case "Writer":
                    ret.Writer ??= new();
                    ret.Writer.Add(crew.Name);
                    break;
            }
        }

        if (sdProgram.Cast is not null)
        {
            foreach (ICast? cast in sdProgram.Cast.OrderBy(a => a.BillingOrder))
            {
                ret.Actor ??= new();
                ret.Actor.Add(new TvActor
                {
                    Text = cast.Name,
                    Role = cast.CharacterName
                });
                break;
            }
        }

        return ret;
    }

    public static TvDesc GetDescriptions(ISDProgram sdProgram, string lang)
    {

        string description = "";
        if (sdProgram.Descriptions is not null)
        {
            if (sdProgram.Descriptions.Description1000 is not null && sdProgram.Descriptions.Description1000.Any())
            {
                IDescription1000? test = sdProgram.Descriptions.Description1000.FirstOrDefault(a => a.DescriptionLanguage == lang && a.Description != "");
                if (test == null)
                {
                    if (!string.IsNullOrEmpty(sdProgram.Descriptions.Description1000[0].Description))
                    {
                        description = sdProgram.Descriptions.Description1000[0].Description;
                    }
                }
                else
                {
                    description = test.Description;
                }
            }

        }

        return new TvDesc
        {
            Lang = lang,
            Text = description
        };
    }

    public static List<TvCategory> GetCategory(ISDProgram sdProgram, string lang)
    {
        List<TvCategory> ret = new();

        if (sdProgram.Genres is not null)
        {
            foreach (string genre in sdProgram.Genres)
            {
                ret.Add(new TvCategory
                {
                    Lang = lang,
                    Text = genre
                });
            }
        }

        return ret;
    }

    public static List<TvEpisodenum> GetEpisodeNums(ISDProgram sdProgram, string lang)
    {
        List<TvEpisodenum> ret = new();
        int season = 0;
        int episode = 0;

        if (sdProgram.Metadata is not null && sdProgram.Metadata.Any())
        {
            foreach (ProgramMetadata m in sdProgram.Metadata.Where(a => a.Gracenote != null))
            {
                season = m.Gracenote.Season;
                episode = m.Gracenote.Episode;
                ret.Add(new TvEpisodenum
                {
                    System = "xmltv_ns",
                    Text = $"{season:00}.{episode:00}"
                });
            }
        }

        if (season != 0 && episode != 0)
        {
            ret.Add(new TvEpisodenum
            {
                System = "onscreen",
                Text = $"S{season} E{episode}"
            });
        }

        if (ret.Count == 0)
        {
            string prefix = sdProgram.ProgramID[..2];
            string newValue;

            switch (prefix)
            {
                case "EP":
                    newValue = sdProgram.ProgramID[..10] + "." + sdProgram.ProgramID[10..];
                    break;

                case "SH":
                case "MV":
                    newValue = sdProgram.ProgramID[..10] + ".0000";
                    break;

                default:
                    newValue = sdProgram.ProgramID;
                    break;
            }
            ret.Add(new TvEpisodenum
            {
                System = "dd_progid",
                Text = newValue
            });
        }

        if (sdProgram.OriginalAirDate != null)
        {
            ret.Add(new TvEpisodenum
            {
                System = "original-air-date",
                Text = sdProgram.OriginalAirDate
            });
        }

        return ret;

    }

    public static List<TvIcon> GetIcons(IProgram program, ISDProgram sdProgram, ISchedule sched, string lang)
    {
        List<TvIcon> ret = new();
        List<string> aspects = new() { "2x3", "4x3", "3x4", "16x9" };

        if (sdProgram.Metadata is not null && sdProgram.Metadata.Any())
        {

        }

        return ret;

    }

    public static List<TvRating> GetRatings(ISDProgram sdProgram, string countryCode, int maxRatings)
    {
        List<TvRating> ratings = new();


        if (sdProgram?.ContentRating == null)
        {

            return ratings;
        }

        maxRatings = maxRatings > 0 ? Math.Min(maxRatings, sdProgram.ContentRating.Count) : sdProgram.ContentRating.Count;

        foreach (ContentRating? cr in sdProgram.ContentRating.Take(maxRatings))
        {
            ratings.Add(new TvRating
            {
                System = cr.Body,
                Value = cr.Code
            });
        }

        return ratings;
    }


    public static TvVideo GetTvVideos(Program sdProgram)
    {
        TvVideo ret = new();

        if (sdProgram.VideoProperties?.Any() == true)
        {
            ret.Quality = sdProgram.VideoProperties.ToList();
        }

        return ret;

    }

    public static TvAudio GetTvAudios(Program sdProgram)
    {
        TvAudio ret = new();

        if (sdProgram.AudioProperties != null && sdProgram.AudioProperties.Any())
        {
            List<string> a = sdProgram.AudioProperties.ToList();
            if (a.Any())
            {
                ret.Stereo = a[0];
            }

        }

        return ret;

    }
    public static SDStatus GetSDStatusOffline()
    {
        SDStatus ret = new();
        ret.systemStatus.Add(new SDSystemStatus { status = "Offline" });
        return ret;
    }
}
