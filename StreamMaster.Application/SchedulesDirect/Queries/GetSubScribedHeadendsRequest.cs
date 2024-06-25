using System.Text.RegularExpressions;

namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSubScribedHeadendsRequest() : IRequest<DataResponse<List<HeadendDto>>>;

internal class GetHeadendsRequestHandler(ISchedulesDirect schedulesDirect, ISender Sender, IOptionsMonitor<SDSettings> intSDSettings)
    : IRequestHandler<GetSubScribedHeadendsRequest, DataResponse<List<HeadendDto>>>
{
    private static readonly Regex FileNamePattern = new Regex(@"Headends-(?<country>[A-Z]{3})-(?<postalCode>\d{5})\.json", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public async Task<HashSet<(string Country, string PostalCode)>> GetCountryAndPostalCodesAsync(string directoryPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        EnumerationOptions enumerationOptions = new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive };
        HashSet<(string Country, string PostalCode)> results = new HashSet<(string Country, string PostalCode)>();

        FileInfo[] files = directoryInfo.GetFiles("Headends-*.json", SearchOption.AllDirectories);
        foreach (var file in files)
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
        List<HeadendDto> ret = new();
        var sdSettings = intSDSettings.CurrentValue;

        var toView = sdSettings.HeadendsToView.Select(a => (a.Country, a.PostalCode)).ToList();

        foreach (var sd in toView)
        {

            var results = await Sender.Send(new GetHeadendsByCountryPostalRequest(sd.Country, sd.PostalCode));
            if (!results.IsError)
            {
                ret.AddRange(results.Data);
            }
        }

        return DataResponse<List<HeadendDto>>.Success(ret);
    }
}
