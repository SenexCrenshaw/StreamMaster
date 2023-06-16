﻿using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities.EPG;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Web;
using System.Xml.Serialization;

using static StreamMasterDomain.Common.GetStreamGroupEPGHandler;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupEPG(int StreamGroupNumber) : IRequest<string>;

public class GetStreamGroupEPGValidator : AbstractValidator<GetStreamGroupEPG>
{
    public GetStreamGroupEPGValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public partial class GetStreamGroupEPGHandler : IRequestHandler<GetStreamGroupEPG, string>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    private readonly ISender _sender;
    private readonly object Lock = new();
    private int dummyCount = 0;

    public GetStreamGroupEPGHandler(
        IMapper mapper, IMemoryCache memoryCache,
        ISender sender,
        IAppDbContext context)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
        _context = context;
        _sender = sender;
    }

    public async Task<string> Handle(GetStreamGroupEPG command, CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<VideoStreamDto> videoStreams = new();
        if (command.StreamGroupNumber > 0)
        {
            StreamGroupDto? sg = await _sender.Send(new GetStreamGroupByStreamNumber(command.StreamGroupNumber), cancellationToken).ConfigureAwait(false);
            if (sg == null)
            {
                return "";
            }
            videoStreams = sg.VideoStreams.Where(a => !a.IsHidden).ToList();
        }
        else
        {
            videoStreams = _context.VideoStreams
                .Where(a => !a.IsHidden)
                .AsNoTracking()
                .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider)
                .ToList();
        }

        ParallelOptions po = new()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = System.Environment.ProcessorCount
        };
        ConcurrentBag<TvChannel> retChannels = new();
        ConcurrentBag<Programme> retProgrammes = new();

        if (videoStreams.Any())
        {
            List<string> epgids = videoStreams.Where(a => !a.IsHidden).Select(a => a.User_Tvg_ID.ToLower()).Distinct().ToList();

            List<Programme> programmes = _memoryCache.Programmes().Where(a => a.Channel != null && epgids.Contains(a.Channel.ToLower())).ToList();

            SettingDto setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

            List<IconFile> progIcons = _context.Icons.Where(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.FileExists).ToList();

            var icons = await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);

            _ = Parallel.ForEach(videoStreams, po, videoStream =>
            {
                if (videoStream == null)
                {
                    return;
                }

                IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == videoStream.User_Tvg_logo);
                string Logo = icon != null ? icon.Source : setting.BaseHostURL + setting.DefaultIcon;

                TvChannel t;
                int dummy = 0;
                if (string.IsNullOrEmpty(videoStream.User_Tvg_ID))
                {
                    videoStream.User_Tvg_ID = "dummy";
                }

                if (videoStream.User_Tvg_ID.ToLower() == "dummy")
                {
                    dummy = GetDummy();

                    t = new TvChannel
                    {
                        Id = videoStream.User_Tvg_ID + "-" + dummy,
                        Icon = new TvIcon { Src = Logo },
                        Displayname = new()
                    {
                    videoStream.User_Tvg_name
                    }
                    };
                }
                else
                {
                    t = new TvChannel
                    {
                        Id = videoStream.User_Tvg_ID.ToString(),
                        Icon = new TvIcon { Src = Logo },
                        Displayname = new()
                    {
                    videoStream.User_Tvg_name
                    }
                    };
                }

                retChannels.Add(t);

                if (videoStream.User_Tvg_ID != null)
                {
                    if (programmes.Any())
                    {
                        IEnumerable<Programme>? progs = programmes.Where(a => a.Channel.ToLower() == videoStream.User_Tvg_ID.ToLower()).DeepCopy();

                        if (progs != null)
                        {
                            foreach (Programme? p in progs)
                            {
                                if (p.Icon.Any())
                                {
                                    foreach (TvIcon progIcon in p.Icon)
                                    {
                                        if (progIcon != null && !string.IsNullOrEmpty(progIcon.Src))
                                        {
                                            IconFile? programmeIcon = progIcons.FirstOrDefault(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == progIcon.Src);

                                            if (programmeIcon == null)
                                            {
                                                continue;
                                            }
                                            string IconSource = $"{setting.BaseHostURL}api/files/{(int)SMFileTypes.ProgrammeIcon}/{HttpUtility.UrlEncode(programmeIcon.Source)}";
                                            progIcon.Src = IconSource;
                                        }
                                    }
                                }
                                else
                                {
                                    p.Icon.Add(new TvIcon { Height = "", Width = "", Src = "" });
                                }
                                p.Channel = videoStream.User_Tvg_ID;
                                if (videoStream.User_Tvg_ID.ToLower() == "dummy")
                                {
                                    p.Channel = videoStream.User_Tvg_ID + "-" + dummy;

                                    p.Title = new TvTitle
                                    {
                                        Lang = "en",
                                        Text = videoStream.User_Tvg_ID,
                                    }; /// channel.Tvg_name;
                                    p.Desc = new TvDesc
                                    {
                                        Lang = "en",
                                        Text = videoStream.User_Tvg_ID,
                                    };
                                }

                                retProgrammes.Add(p);
                            }
                        }
                    }
                }
            });
        }

        Tv ret = new()
        {
            Channel = retChannels.ToList(),
            Programme = retProgrammes.ToList()
        };

        XmlSerializerNamespaces ns = new();
        ns.Add("", "");

        using Utf8StringWriter textWriter = new();
        XmlSerializer serializer = new(typeof(Tv));
        serializer.Serialize(textWriter, ret, ns);
        textWriter.Close();
        sw.Stop();
        long el = sw.ElapsedMilliseconds;
        return textWriter.ToString();
    }

    private int GetDummy()
    {
        lock (Lock)
        {
            ++dummyCount;
            return dummyCount;
        }
    }
}
