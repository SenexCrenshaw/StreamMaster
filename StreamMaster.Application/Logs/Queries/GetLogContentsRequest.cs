using System.Text.Json;

namespace StreamMaster.Application.Logs.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetLogContentsRequest(string logName) : IRequest<DataResponse<string>>;

public class GetLogContentsRequestHandler()
    : IRequestHandler<GetLogContentsRequest, DataResponse<string>>
{
    public async Task<DataResponse<string>> Handle(GetLogContentsRequest request, CancellationToken cancellationToken = default)
    {
        DirectoryInfo DirectoryInfo = new(BuildInfo.LogFolder);
        if (!DirectoryInfo.Exists)
        {
            return DataResponse<string>.Success("");
        }
        string filePath = Path.Combine(BuildInfo.LogFolder, request.logName);
        if (!File.Exists(filePath))
        {
            return DataResponse<string>.Success("");
        }
        string logContents = await File.ReadAllTextAsync(filePath, cancellationToken);
        string jsonString = JsonSerializer.Serialize(logContents);
        return DataResponse<string>.Success(jsonString);
    }
}