using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Web;

namespace StreamMasterApplication.Icons.Commands;

public class CacheIconsFromProgrammesRequest : IRequest<bool>
{
}

public class CacheIconsFromProgrammesRequestHandler : IRequestHandler<CacheIconsFromProgrammesRequest, bool>
{
    private readonly IAppDbContext _context;

    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CacheIconsFromProgrammesRequestHandler> _logger;
    private readonly ISender _sender;

    public CacheIconsFromProgrammesRequestHandler(
        ILogger<CacheIconsFromProgrammesRequestHandler> logger,
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

    public async Task<bool> Handle(CacheIconsFromProgrammesRequest command, CancellationToken cancellationToken)
    {
        SettingDto _setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

        var icons = new List<string>();

        var epgFiles=_context.EPGFiles.ToList();
        foreach(var epg in epgFiles)
        {
            var tv = await epg.GetTV();
            if ( tv == null)
            {
                continue;
            }

            icons = tv.Channel
                .Where(a => a is not null && a.Icon is not null  && !string.IsNullOrEmpty(a.Icon.Src))
                .Select(a => a.Icon.Src)
                .ToList();

            if (!icons.Any()) { continue; }

            await WorkOnIcons(FileDefinitions.Icon, icons, _setting, cancellationToken).ConfigureAwait(false);
        }

        icons = new List<string>();

        string token = "";

        

        IEnumerable<StreamMasterDomain.Entities.EPG.Programme> _programmes = _memoryCache.Programmes();

        var icons2 = _programmes.Where(a => a is not null && a.Icon is not null && a.Icon.Count > 0)
             .SelectMany(a => a.Icon.Select(a => a.Src));

         icons = _programmes
            .Where(a => a is not null && a.Icon is not null && a.Icon.Count > 0 && a.Icon.Any(a => a.Src is not null))
            .SelectMany(a => a.Icon.Where(a => a.Src is not null).Select(a => a.Src))
            .Distinct().ToList();

        if (!icons.Any()) { return false; }

        await WorkOnIcons(FileDefinitions.ProgrammeIcon,icons, _setting, cancellationToken).ConfigureAwait(false);
       
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
                Console.WriteLine($"CacheIconsFromProgrammes {count} of {icons.Count}");
            }

            string source = HttpUtility.UrlDecode(icon);
            string tocheck = source;

            if (tocheck.ToLower().StartsWith("https://json.schedulesdirect.org/20141201/image/"))
            {
                if (token == "")
                {
                    using HttpClient httpClient = new();

                    SDGetTokenRequest data = new()
                    {
                        username = setting.SDUserName,
                        password = setting.SDPassword
                    };

                    string jsonString = JsonSerializer.Serialize(data);
                    StringContent content = new(jsonString, Encoding.UTF8, "application/json");
                    string userAgentString = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.57";

                    httpClient.DefaultRequestHeaders.Add("User-Agent", userAgentString);
                    using HttpResponseMessage response = await httpClient.PostAsync("https://json.schedulesdirect.org/20141201/token", content, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        _ = response.EnsureSuccessStatusCode();
                        string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        SDGetToken? result = JsonSerializer.Deserialize<SDGetToken>(responseString);
                        if (result == null || string.IsNullOrEmpty(result.token))
                        {
                            continue;
                        }
                        token = result.token;

                    }
                    catch (HttpRequestException ex)
                    {
                        //_logger.LogCritical(ex, "Error while retieving icon");                        
                    }
                    if (string.IsNullOrEmpty(token))
                {
                    continue;
                }

                string name = Path.GetFileNameWithoutExtension(tocheck);
                (_, isNew) = await IconHelper.AddIcon(tocheck, "?token=" + token, name, _context, _mapper, setting, fd, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                string name = Path.GetFileNameWithoutExtension(tocheck);
                (_, isNew) = await IconHelper.AddIcon(tocheck, null, name, _context, _mapper, setting, fd, cancellationToken).ConfigureAwait(false);
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
