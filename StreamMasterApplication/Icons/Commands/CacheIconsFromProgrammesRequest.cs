using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;

using System.Web;

namespace StreamMasterApplication.Icons.Commands;

public class CacheIconsFromEPGsRequest : IRequest<bool>
{
}

public class CacheIconsFromEPGsRequestHandler : IRequestHandler<CacheIconsFromEPGsRequest, bool>
{
    private readonly IAppDbContext _context;

    private readonly ILogger<CacheIconsFromEPGsRequestHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ISender _sender;

    public CacheIconsFromEPGsRequestHandler(
        ILogger<CacheIconsFromEPGsRequestHandler> logger,
        IMemoryCache memoryCache,
          IMapper mapper,

         IAppDbContext context, ISender sender)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _mapper = mapper;
        _context = context;
        _sender = sender;
    }

    public async Task<bool> Handle(CacheIconsFromEPGsRequest command, CancellationToken cancellationToken)
    {
        return false;
        SettingDto _setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);
        if (!_setting.CacheIcons)
        {
            return false;
        }

        var icons = new List<string>();

        var epgFiles = _context.EPGFiles.ToList();
        foreach (var epg in epgFiles)
        {
            var tv = await epg.GetTV();
            if (tv == null)
            {
                continue;
            }

            icons = tv.Channel
                .Where(a => a is not null && a.Icon is not null && !string.IsNullOrEmpty(a.Icon.Src))
                .Select(a => a.Icon.Src)
                .ToList();

            if (!icons.Any()) { continue; }

            await WorkOnIcons(FileDefinitions.Icon, icons, _setting, cancellationToken).ConfigureAwait(false);
        }

        IEnumerable<StreamMasterDomain.Entities.EPG.Programme> _programmes = _memoryCache.Programmes();

        icons = _programmes
           .Where(a => a is not null && a.Icon is not null && a.Icon.Count > 0 && a.Icon.Any(a => a.Src is not null))
           .SelectMany(a => a.Icon.Where(a => a.Src is not null).Select(a => a.Src))
           .Distinct().ToList();

        var test = icons.FirstOrDefault(a => a.Contains("deba6af644347122056ec73f6b885215ff4534230b214addfc795ae7db60c38f"));

        if (!icons.Any()) { return false; }

        await WorkOnIcons(FileDefinitions.ProgrammeIcon, icons, _setting, cancellationToken).ConfigureAwait(false);

        return true;
    }

    private async Task<bool> WorkOnIcons(FileDefinition fd, List<string> icons, SettingDto setting, CancellationToken cancellationToken)
    {
        int count = 0;
        var isNew = false;
        string token = "";

        foreach (string? icon in icons)
        {
            if (icon is null)
            {
                continue;
            }

            if (cancellationToken.IsCancellationRequested) { return false; }

            ++count;
            if (count % 100 == 0)
            {
                Console.WriteLine($"CacheIconsFromEPGs {count} of {icons.Count}");
            }

            //if (icon.Contains("deba6af644347122056ec73f6b885215ff4534230b214addfc795ae7db60c38f"))
            //{
            //    var aaa = 1;
            //}

            string source = HttpUtility.UrlDecode(icon);
            string tocheck = source;

            if (tocheck.ToLower().StartsWith("https://json.schedulesdirect.org/20141201/image/"))
            {
                //var sd = new SchedulesDirect();
                //if (await sd.CheckToken())
                //{
                //    token = sd.Token;

                //if (token == "")
                //{
                //    using HttpClient httpClient = new();

                // SDGetTokenRequest data = new() { username =
                // setting.SDUserName, password = setting.SDPassword };

                // string jsonString = JsonSerializer.Serialize(data);
                // StringContent content = new(jsonString, Encoding.UTF8,
                // "application/json"); string userAgentString = @"Mozilla/5.0
                // (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like
                // Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.57";

                //    httpClient.DefaultRequestHeaders.Add("User-Agent", userAgentString);
                //    using HttpResponseMessage response = await httpClient.PostAsync("https://json.schedulesdirect.org/20141201/token", content, cancellationToken).ConfigureAwait(false);
                //    try
                //    {
                //        _ = response.EnsureSuccessStatusCode();
                //        string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                //        SDGetToken? result = JsonSerializer.Deserialize<SDGetToken>(responseString);
                //        if (result == null || string.IsNullOrEmpty(result.token))
                //        {
                //            continue;
                //        }
                //        token = result.token;
                //    }
                //    catch (HttpRequestException ex)
                //    {
                //        //_logger.LogCritical(ex, "Error while retieving icon");
                //    }
                //    if (string.IsNullOrEmpty(token))
                //    {
                //        continue;
                //    }
                //}

                // FIX ME
                //string name = Path.GetFileNameWithoutExtension(tocheck);
                //(_, isNew) = await IconHelper.AddIcon(tocheck, "?token=" + token, name, _context, _mapper, setting, fd, cancellationToken).ConfigureAwait(false);
                //}
            }
            else
            {
                string name = Path.GetFileNameWithoutExtension(tocheck);
                await IconHelper.AddIcon(tocheck, name, _mapper, _memoryCache, fd, cancellationToken);
            }
        }

        return isNew;
    }

    private class SDGetToken
    {
        public int code { get; set; }
        public DateTime datetime { get; set; }
        public string? message { get; set; }
        public string? serverID { get; set; }
        public string? token { get; set; }
    }

    private class SDGetTokenRequest
    {
        public string? password { get; set; }
        public string? username { get; set; }
    }
}
