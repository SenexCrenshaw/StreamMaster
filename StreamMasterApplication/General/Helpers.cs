using StreamMasterDomain.Dto;

using System.Web;

namespace StreamMasterApplication.General;

public static class Helpers
{
    public static string GetIPTVChannelIconSources(string IconSource, SettingDto setting, IEnumerable<IconFileDto> Icons)
    {
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
            return setting.BaseHostURL + IconSource;
        }

        if (IconSource.Equals(setting.StreamMasterIcon))
        {
            return setting.BaseHostURL + IconSource;
        }

        if (IconSource.StartsWith("api/files/"))
        {
            return $"{setting.BaseHostURL}{IconSource}";
        }

        if (!Icons.Any())
        {
            IconSource = setting.BaseHostURL + Constants.IconDefault;
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
            IconSource = setting.BaseHostURL + Constants.IconDefault;
            return IconSource;
        }

        if (icon.FileExists)
        {
            if (setting.CacheIcons)
            {
                IconSource = $"{setting.BaseHostURL}api/files/{(int)iptvFileType}/{HttpUtility.UrlEncode(toMatch)}";
            }
            else
            {
                IconSource = iptvFileType == SMFileTypes.Icon ?
                Constants.IconDefault :
                    $"{setting.BaseHostURL}api/files/{(int)iptvFileType}/{HttpUtility.UrlEncode(icon.Source)}";
            }
        }
        else
        {
            IconSource = setting.BaseHostURL + Constants.IconDefault;
        }
        return IconSource;
    }
}
