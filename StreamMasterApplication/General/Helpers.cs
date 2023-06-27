using StreamMasterDomain.Dto;

using System.Web;

namespace StreamMasterApplication.General;

public static class Helpers
{
    public static string GetIPTVChannelIconSources(string IconSource, SettingDto setting, string baseHostURL, IEnumerable<IconFileDto> Icons)
    {
        //var baseHostURL = setting.BaseHostURL;

        //if (!Debugger.IsAttached)
        //    baseHostURL = "/";

        if (!setting.CacheIcons)
        {
            return IconSource;
        }

        if (string.IsNullOrEmpty(IconSource))
        {
            return IconSource;
        }

        if (IconSource.Equals(setting.DefaultIcon))
        {
            return baseHostURL + IconSource;
        }

        if (IconSource.Equals(setting.StreamMasterIcon))
        {
            return baseHostURL + IconSource;
        }

        if (IconSource.StartsWith("api/files/"))
        {
            return $"{baseHostURL}{IconSource}";
        }

        if (!Icons.Any())
        {
            IconSource = "/" + Constants.IconDefault;
            return IconSource;
        }

        SMFileTypes iptvFileType = SMFileTypes.Icon;

        string toMatch = "";

        IconFileDto? icon = Icons.FirstOrDefault(a => a.Source == IconSource);
        if (icon is null)
        {
            icon = Icons.FirstOrDefault(a => a.Name == IconSource);

            if (icon is not null)
            {
                iptvFileType = SMFileTypes.TvLogo;
                string ext = Path.GetExtension(icon.OriginalSource);
                toMatch = icon.Name + ext;
            }
        }
        else
        {
            toMatch = icon.Source;
        }

        if (icon is null)
        {
            IconSource = baseHostURL + Constants.IconDefault;
            return IconSource;
        }

        if (icon.FileExists)
        {
            if (setting.CacheIcons)
            {
                IconSource = $"{baseHostURL}api/files/{(int)iptvFileType}/{HttpUtility.UrlEncode(toMatch)}";
            }
            else
            {
                IconSource = iptvFileType == SMFileTypes.Icon ?
                Constants.IconDefault :
                    $"{baseHostURL}api/files/{(int)iptvFileType}/{HttpUtility.UrlEncode(icon.Source)}";
            }
        }
        else
        {
            IconSource = baseHostURL + Constants.IconDefault;
        }
        return IconSource;
    }
}
