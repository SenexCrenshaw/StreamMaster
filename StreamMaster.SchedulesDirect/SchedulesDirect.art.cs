using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;

namespace StreamMaster.SchedulesDirect;
public partial class SchedulesDirect
{
    public async Task<List<string>?> GetCustomLogosFromServerAsync(string server)
    {
        return await schedulesDirectAPI.GetApiResponse<List<string>>(APIMethod.GET, server);
    }

    //private void UpdateMovieIcons(List<MxfProgram> mxfPrograms)
    //{
    //    foreach (MxfProgram? prog in mxfPrograms.Where(a => a.Extras.ContainsKey("artwork")))
    //    {
    //        List<ProgramArtwork> artwork = prog.Extras["artwork"];
    //        UpdateIcons(artwork.Select(a => a.Uri), prog.Title);
    //    }
    //}

    //private void UpdateIcons(List<MxfProgram> mxfPrograms)
    //{
    //    foreach (MxfProgram? prog in mxfPrograms.Where(a => a.Extras.ContainsKey("artwork")))
    //    {
    //        List<ProgramArtwork> artwork = prog.Extras["artwork"];
    //        UpdateIcons(artwork.Select(a => a.Uri), prog.Title);
    //    }
    //}

    //private void UpdateSeasonIcons(List<Season> mxfSeasons)
    //{
    //    foreach (Season? prog in mxfSeasons.Where(a => a.Extras.ContainsKey("artwork")))
    //    {
    //        List<ProgramArtwork> artwork = prog.Extras["artwork"];
    //        UpdateIcons(artwork.Select(a => a.Uri), prog.Title);
    //    }
    //}
    //private void UpdateIcons(List<SeriesInfo> mxfSeriesInfos)
    //{
    //    foreach (SeriesInfo? prog in mxfSeriesInfos.Where(a => a.Extras.ContainsKey("artwork")))
    //    {
    //        List<ProgramArtwork> artwork = prog.Extras["artwork"];
    //        UpdateIcons(artwork.Select(a => a.Uri), prog.Title);
    //    }
    //}
    private void AddIcon(string artworkUri, string title)
    {
        if (string.IsNullOrEmpty(artworkUri))
        {
            return;
        }

        List<LogoFileDto> icons = logoService.GetLogos();

        if (icons.Any(a => a.SMFileType == SMFileTypes.SDImage && a.Source == artworkUri))
        {
            return;
        }

        logoService.AddLogo(new LogoFileDto { Source = artworkUri, SMFileType = SMFileTypes.SDImage, Name = title });
    }

    public void UpdateIcons(IEnumerable<string> artworkUris, string title)
    {
        if (!artworkUris.Any())
        {
            return;
        }

        List<LogoFileDto> icons = logoService.GetLogos(SMFileTypes.SDImage);

        foreach (string artworkUri in artworkUris)
        {
            if (icons.Any(a => a.Source == artworkUri))
            {
                continue;
            }
            AddIcon(artworkUri, title);
        }
    }
}