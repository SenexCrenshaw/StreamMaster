
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace StreamMaster.SchedulesDirectAPI.Helpers;

public static partial class SDHelpers
{

    public static bool TableContains(string[] table, string text, bool exactMatch = false)
    {
        if (table == null) return false;
        foreach (var str in table)
        {
            if (string.IsNullOrEmpty(str)) continue;
            if (!exactMatch && str.ToLower().Contains(text.ToLower())) return true;
            if (str.ToLower().Equals(text.ToLower())) return true;
        }

        return false;
    }

    public static bool StringContains(this string str, string text)
    {
        return str != null && str.ToLower().Contains(text.ToLower());
    }

    public static string ReportExceptionMessages(Exception ex)
    {
        var ret = string.Empty;
        var innerException = ex;
        do
        {
            ret += $" {innerException.Message} ";
            innerException = innerException.InnerException;
        } while (innerException != null);
        return ret;
    }

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
        })
        {
            Timeout = TimeSpan.FromMinutes(5)
        };
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd(clientUserAgent);
        client.DefaultRequestHeaders.ExpectContinue = true;

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

    public static UserStatus GetStatusOffline()
    {
        UserStatus ret = new();
        ret.SystemStatus.Add(new SystemStatus { Status = "Offline" });
        return ret;
    }

    //public static List<TvTitle> GetTitles(List<Title> Titles, string lang)
    //{
    //    return Titles.ConvertAll(a => new TvTitle
    //    {
    //        Lang = lang,
    //        Text = a.Title120
    //    });
    //}

    //public static TvSubtitle GetSubTitles(Programme sdProgram, string lang)
    //{
    //    if (!string.IsNullOrEmpty(sdProgram.EpisodeTitle150))
    //    {
    //        return new TvSubtitle
    //        {
    //            Lang = lang,
    //            Text = sdProgram.EpisodeTitle150
    //        };
    //    }

    //    string description = "";
    //    if (sdProgram.Descriptions?.Description100 is not null)
    //    {
    //        IDescription100? test = sdProgram.Descriptions.Description100.Find(a => a.DescriptionLanguage == lang && a.Description != "");
    //        if (test == null)
    //        {
    //            if (!string.IsNullOrEmpty(sdProgram.Descriptions.Description100[0].Description))
    //            {
    //                description = sdProgram.Descriptions.Description100[0].Description;
    //            }
    //        }
    //        else
    //        {
    //            description = test.Description;
    //        }
    //    }

    //    return new TvSubtitle
    //    {
    //        Lang = lang,
    //        Text = description
    //    };
    //}

    //public static TvCredits GetCredits(Programme sdProgram)
    //{
    //    TvCredits ret = new()
    //    {
    //        Actors = [],
    //        Directors = [],
    //        Producers = [],
    //        Presenters = [],
    //        Writers = [],
    //        Adapters = [],
    //        Composers = [],
    //        Editors = [],
    //        Commentators = [],
    //        Guests = []
    //    };

    //    if (sdProgram.Crew == null)
    //    {
    //        return ret;
    //    }
    //    List<string> roles = sdProgram.Crew.Select(a => a.Role).Distinct().ToList();
    //    string rolesName = string.Join(',', roles);
    //    foreach (Crew crew in sdProgram.Crew)
    //    {
    //        switch (crew.Role)
    //        {
    //            case "Actor":
    //                ret.Actors.Add(new TvActor
    //                {
    //                    Text = crew.Name,
    //                    Role = crew.Role
    //                });
    //                break;
    //            case "Director":
    //                ret.Directors.Add(crew.Name);
    //                break;
    //            case "Adapter":
    //                ret.Adapters.Add(crew.Name);
    //                break;
    //            case "Producer":
    //                ret.Producers.Add(crew.Name);
    //                break;
    //            case "Composer":
    //                ret.Composers.Add(crew.Name);
    //                break;
    //            case "Editor":
    //                ret.Editors.Add(crew.Name);
    //                break;
    //            case "Presenter":
    //                ret.Presenters.Add(crew.Name);
    //                break;
    //            case "Writer":
    //                ret.Writers.Add(crew.Name);
    //                break;
    //            case "Commentator":
    //                ret.Commentators.Add(crew.Name);
    //                break;
    //            case "Guest":
    //                ret.Guests.Add(crew.Name);
    //                break;
    //        }
    //    }

    //    if (sdProgram.Cast is not null)
    //    {
    //        foreach (ICast? cast in sdProgram.Cast.OrderBy(a => a.BillingOrder))
    //        {
    //            ret.Actors.Add(new TvActor
    //            {
    //                Text = cast.Name,
    //                Role = cast.CharacterName
    //            });
    //        }
    //    }

    //    return ret;
    //}

    //public static TvDesc GetDescriptions(Programme sdProgram, string lang)
    //{
    //    string description = "";
    //    if (sdProgram.Descriptions is not null)
    //    {
    //        if (sdProgram.Descriptions.Description1000?.Any() == true)
    //        {
    //            IDescription1000? test = sdProgram.Descriptions.Description1000.Find(a => a.DescriptionLanguage == lang && a.Description != "");
    //            if (test == null)
    //            {
    //                if (!string.IsNullOrEmpty(sdProgram.Descriptions.Description1000[0].Description))
    //                {
    //                    description = sdProgram.Descriptions.Description1000[0].Description;
    //                }
    //            }
    //            else
    //            {
    //                description = test.Description;
    //            }
    //        }
    //    }
    //    TvSubtitle subTitle = GetSubTitles(sdProgram, lang);

    //    if (!string.IsNullOrEmpty(subTitle.Text) && !string.IsNullOrEmpty(description))
    //    {
    //        description = $"[{subTitle.Text}]\n" + description;
    //    }

    //    return new TvDesc
    //    {
    //        Lang = lang,
    //        Text = description
    //    };
    //}

    //public static List<TvCategory> GetCategory(Programme sdProgram, string lang)
    //{
    //    List<TvCategory> ret = [];

    //    if (sdProgram.Genres is not null)
    //    {
    //        foreach (string genre in sdProgram.Genres)
    //        {
    //            ret.Add(new TvCategory
    //            {
    //                Lang = lang,
    //                Text = genre
    //            });
    //        }
    //    }

    //    return ret;
    //}

    //public static List<TvEpisodenum> GetEpisodeNums(Programme sdProgram)
    //{
    //    List<TvEpisodenum> ret = [];
    //    int season = 0;
    //    int episode = 0;

    //    if (sdProgram.Metadata?.Any() == true)
    //    {
    //        foreach (ProgramMetadata m in sdProgram.Metadata.Where(a => a.Gracenote != null))
    //        {
    //            season = m.Gracenote.Season;
    //            episode = m.Gracenote.Episode;
    //            ret.Add(new TvEpisodenum
    //            {
    //                System = "xmltv_ns",
    //                Text = $"{season}.{episode}."
    //            });
    //        }
    //    }

    //    if (season != 0 && episode != 0)
    //    {
    //        ret.Add(new TvEpisodenum
    //        {
    //            System = "onscreen",
    //            Text = $"S{season} E{episode}"
    //        });
    //    }

    //    if (ret.Count == 0)
    //    {
    //        string prefix = sdProgram.ProgramID[..2];
    //        string newValue = prefix switch
    //        {
    //            "EP" => sdProgram.ProgramID[..10] + "." + sdProgram.ProgramID[10..],
    //            "SH" or "MV" => sdProgram.ProgramID[..10] + ".0000",
    //            _ => sdProgram.ProgramID,
    //        };
    //        ret.Add(new TvEpisodenum
    //        {
    //            System = "dd_progid",
    //            Text = newValue
    //        });
    //    }

    //    if (sdProgram.OriginalAirDate != null)
    //    {
    //        ret.Add(new TvEpisodenum
    //        {
    //            System = "original-air-date",
    //            Text = sdProgram.OriginalAirDate
    //        });
    //    }

    //    return ret;
    //}

    //public static List<TvIcon> GetIcons(Programme sdProgram, IMemoryCache memoryCache)
    //{
    //    List<ImageInfo> imageInfos = memoryCache.ImageInfos();
    //    IEnumerable<ImageInfo> icons = imageInfos.Where(a => a.ProgramId == sdProgram.ProgramID);
    //    if (!icons.Any())
    //    {
    //        return [];
    //    }

    //    List<TvIcon> ret = icons.Select(a => new TvIcon
    //    {
    //        Src = a.RealUrl,
    //        Width = a.Width.ToString(),
    //        Height = a.Height.ToString()
    //    }).ToList();

    //    return ret;
    //}

    //public static List<TvRating> GetRatings(Programme sdProgram, int maxRatings)
    //{
    //    List<TvRating> ratings = [];

    //    if (sdProgram?.ContentRating == null)
    //    {
    //        return ratings;
    //    }

    //    maxRatings = maxRatings > 0 ? Math.Min(maxRatings, sdProgram.ContentRating.Count) : sdProgram.ContentRating.Count;

    //    foreach (ContentRating? cr in sdProgram.ContentRating.Take(maxRatings))
    //    {
    //        ratings.Add(new TvRating
    //        {
    //            System = cr.Body,
    //            Value = cr.Code
    //        });
    //    }

    //    return ratings;
    //}

    //public static TvVideo GetTvVideos(Program sdProgram)
    //{
    //    TvVideo ret = new();

    //    if (sdProgram.VideoProperties?.Any() == true)
    //    {
    //        ret.Quality = [.. sdProgram.VideoProperties];
    //    }

    //    return ret;
    //}

    //public static TvAudio GetTvAudios(Program sdProgram)
    //{
    //    TvAudio ret = new();

    //    if (sdProgram.AudioProperties?.Any() == true)
    //    {
    //        List<string> a = [.. sdProgram.AudioProperties];
    //        if (a.Any())
    //        {
    //            ret.Stereo = a.Last() switch
    //            {
    //                "stereo" => "stereo",
    //                "dvs" => "stereo",
    //                "DD 5.1" => "dolby digital",
    //                "Atmos" => "dolby digital",
    //                "Dolby" => "dolby",
    //                "dubbed" => "mono",
    //                "mono" => "mono",
    //                _ => "mono",
    //            };
    //        }
    //    }

    //    return ret;
    //}

    //public static TvPreviouslyshown GetPreviouslyShown(Programme sdProgram)
    //{
    //    return new()
    //    {
    //        Start = sdProgram.OriginalAirDate ?? ""
    //    };
    //}
    //public static string? GetNew(Program sdProgram)
    //{
    //    return sdProgram.New != null ? "" : null;
    //}

}
