using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Domain.Color;

namespace StreamMaster.Application.EPGFiles;

public class EPGFileService(IRepositoryWrapper repositoryWrapper, IFileUtilService fileUtilService) : IEPGFileService
{
    public async Task<(EPGFile epgFile, string fullName)> CreateEPGFileAsync(CreateEPGFileRequest request)
    {
        return await CreateEPGFileBase(request.Name, request.UrlSource, request.Color, request.EPGNumber, request.HoursToUpdate, request.TimeShift);
    }

    public async Task<List<EPGFile>> GetEPGFilesAsync()
    {
        return await repositoryWrapper.EPGFile.GetQuery().ToListAsync();
    }

    public async Task<(EPGFile epgFile, string fullName)> CreateEPGFileAsync(CreateEPGFileFromFormRequest request)
    {
        return await CreateEPGFileBase(request.Name, null, request.Color, request.EPGNumber, request.HoursToUpdate, request.TimeShift);
    }

    public async Task GetProgramsFromEPG()
    {
        List<EPGFile> epgFiles = await GetEPGFilesAsync();
        List<StationChannelName> channelNames = [];// schedulesDirectDataService.GetStationChannelNames().ToList();

        foreach (EPGFile epgFile in epgFiles)
        {
            string epgPath = Path.Combine(FileDefinitions.EPG.DirectoryLocation, epgFile.Source);

            List<StationChannelName>? newNames = await fileUtilService.ProcessStationChannelNamesAsync(epgPath, epgFile.EPGNumber);
            if (newNames is not null)
            {
                channelNames.AddRange(newNames);
            }
        }
    }

    private async Task<(EPGFile epgFile, string fullName)> CreateEPGFileBase(string name, string? UrlSource, string? Color, int? EPGNumber, int? HoursToUpdate, int? TimeShift)
    {
        int num = EPGNumber ?? 0;

        if (num == 0 || !EPGNumber.HasValue || await repositoryWrapper.EPGFile.GetEPGFileByNumber(EPGNumber.Value).ConfigureAwait(false) != null)
        {
            num = await repositoryWrapper.EPGFile.GetNextAvailableEPGNumberAsync(CancellationToken.None).ConfigureAwait(false);
        }

        (string fullName, string fullNameWithExtension) = GetFileName(name);

        EPGFile epgFile = new()
        {
            Name = name,
            Url = UrlSource ?? "",
            Source = fullNameWithExtension,
            Color = Color ?? ColorHelper.GetColor(name),
            EPGNumber = num,
            HoursToUpdate = HoursToUpdate ?? 72,
            TimeShift = TimeShift ?? 0
        };

        return (epgFile, fullName);
    }

    public async Task<DataResponse<List<EPGFileDto>>> GetEPGFilesNeedUpdatingAsync()
    {
        List<EPGFileDto> epgFiles = await repositoryWrapper.EPGFile.GetEPGFilesNeedUpdatingAsync();
        return DataResponse<List<EPGFileDto>>.Success(epgFiles);
    }

    public (string fullName, string fullNameWithExtension) GetFileName(string name)
    {
        FileDefinition fd = FileDefinitions.EPG;

        string fullNameWithExtension = name + fd.DefaultExtension;
        string compressedFileName = fileUtilService.CheckNeedsCompression(fullNameWithExtension);
        string fullName = Path.Combine(fd.DirectoryLocation, compressedFileName);
        return (fullName, fullNameWithExtension);
    }
}