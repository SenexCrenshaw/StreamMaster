using System.Text.RegularExpressions;

namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSubScribedHeadendsRequest() : IRequest<DataResponse<List<HeadendDto>>>;

internal partial class GetSubScribedHeadendsRequestHandler(ISender Sender, IOptionsMonitor<SDSettings> intSDSettings)
    : IRequestHandler<GetSubScribedHeadendsRequest, DataResponse<List<HeadendDto>>>
{
    private static readonly Regex FileNamePattern = MyRegex();

    public static HashSet<(string Country, string PostalCode)> GetCountryAndPostalCodes(string directoryPath)
    {
        DirectoryInfo directoryInfo = new(directoryPath);
        //EnumerationOptions enumerationOptions = new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive };
        HashSet<(string Country, string PostalCode)> results = [];

        FileInfo[] files = directoryInfo.GetFiles("Headends-*.json", SearchOption.AllDirectories);
        foreach (FileInfo file in files)
        {
            Match match = FileNamePattern.Match(file.Name);
            if (match.Success)
            {
                string country = match.Groups["country"].Value;
                string postalCode = match.Groups["postalCode"].Value;
                results.Add((country, postalCode));
            }
        }

        return results;
    }

    public async Task<DataResponse<List<HeadendDto>>> Handle(GetSubScribedHeadendsRequest request, CancellationToken cancellationToken)
    {
        List<HeadendDto> ret = [];
        SDSettings sdSettings = intSDSettings.CurrentValue;

        List<(string Country, string PostalCode)> toView = sdSettings.HeadendsToView.ConvertAll(a => (a.Country, a.PostalCode));

        foreach ((string Country, string PostalCode) sd in toView)
        {
            DataResponse<List<HeadendDto>> results = await Sender.Send(new GetHeadendsByCountryPostalRequest(sd.Country, sd.PostalCode), cancellationToken);
            if (!results.IsError && results.Data is not null)
            {
                ret.AddRange(results.Data);
            }
        }

        return DataResponse<List<HeadendDto>>.Success(ret);
    }

    [GeneratedRegex(@"Headends-(?<country>[A-Z]{3})-(?<postalCode>\d{5})\.json", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex MyRegex();
}
