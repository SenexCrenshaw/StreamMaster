namespace StreamMaster.Application.Logs.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetLogNamesRequest() : IRequest<DataResponse<List<string>>>;

public class GetLogNamesRequestHandler()
    : IRequestHandler<GetLogNamesRequest, DataResponse<List<string>>>
{
    public async Task<DataResponse<List<string>>> Handle(GetLogNamesRequest request, CancellationToken cancellationToken = default)
    {
        DirectoryInfo DirectoryInfo = new(BuildInfo.LogFolder);
        if (!DirectoryInfo.Exists)
        {
            return DataResponse<List<string>>.Success([]);
        }
        //string path = Path.Combine(BuildInfo.LogFolder, BuildInfo.LogFileName + "*.log");

        List<string> logFiles = [.. DirectoryInfo.GetFiles(BuildInfo.LogFileName + "*.log")
       .Select(a => a.Name)
       .OrderBy(GetLogFileSortKey)];

        return await Task.FromResult(DataResponse<List<string>>.Success(logFiles));
    }

    // Helper method to generate sort key
    private static (int order, int? number) GetLogFileSortKey(string logFile)
    {
        // Check if it's the base log file (StreamMasterAPI.log)
        if (logFile == "StreamMasterAPI.log")
        {
            return (0, null); // Base file should come first, order 0
        }

        // Match the numeric suffix using the pattern: StreamMasterAPI.X.log
        string[] parts = logFile.Split('.');
        if (parts.Length == 3 && int.TryParse(parts[1], out int logNumber))
        {
            return (1, logNumber); // Rotated logs have order 1, sorted by number
        }

        return (2, null); // If any other pattern, it comes last
    }
}