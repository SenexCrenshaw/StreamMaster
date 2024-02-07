using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Logging;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Services;
using StreamMaster.Infrastructure;
using StreamMaster.Infrastructure.EF;
using StreamMaster.Infrastructure.EF.PGSQL;
using StreamMaster.SchedulesDirect;
using StreamMaster.SchedulesDirect.Converters;
using StreamMaster.SchedulesDirect.Data;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.XmltvXml;

namespace TestCreator
{
    internal class Program
    {
        private static ILogger<Program>? _logger;

        private static void Main()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddLogging(configure => configure.AddConsole())
                .AddSingleton<ISchedulesDirectDataService, SchedulesDirectDataService>()
                .AddTransient<IXmltv2Mxf, XmlTv2Mxf>()
                .AddSingleton<IXMLTVBuilder, XMLTVBuilder>()
                .AddInfrastructureServices()
                .AddInfrastructureEFServices()
                .BuildServiceProvider();

            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            GlobalLoggerProvider.Configure(loggerFactory);
            _logger = serviceProvider.GetService<ILogger<Program>>()!;


            IXmltv2Mxf? xmltv2Mxf = serviceProvider.GetService<IXmltv2Mxf>()!;

            string fullName = "C:\\config\\PlayLists\\epg123.xml";
            if (File.Exists(fullName) == false)
            {
                _logger.LogInformation("File {fullName} does not exist", fullName);
                return;
            }

            XMLTV? epgData = xmltv2Mxf.ConvertToMxf(fullName, 0);
            if (epgData == null)
            {
                _logger.LogCritical("Exception EPG {fullName} format is not supported", fullName);
                return;
            }

            List<string> channelNames = epgData.Programs.Select(a => a.Channel).Distinct().ToList();

            PGSQLRepositoryContext? context = serviceProvider.GetService<PGSQLRepositoryContext>();
            if (context == null)
            {
                _logger.LogCritical("RepositoryContext is null");
                return;
            }
            StreamGroup sg = context.StreamGroups.First(a => a.Id == 4);

            context.StreamGroupVideoStreams.RemoveRange(context.StreamGroupVideoStreams.Where(a => a.StreamGroupId == 4));

            context.VideoStreams.RemoveRange(context.VideoStreams
                .Where(a => a.User_Tvg_group == "LOCAL CHANNELS - USA" && a.User_Tvg_name.StartsWith("Tester")));

            context.SaveChanges();

            Console.WriteLine($"Creating {channelNames.Count} test streams");
            int count = 0;
            foreach (string a in channelNames)
            {
                ++count;
                VideoStream videoStream = new()
                {
                    Id = IdConverter.GetID(),
                    IsUserCreated = true,
                    M3UFileName = "CUSTOM",
                    Tvg_group = "TESTING",
                    User_Tvg_group = "TESTING",
                    Tvg_ID = $"2-{a}",
                    User_Tvg_ID = $"2-{a}",
                    Tvg_name = $"Test {a}",
                    User_Tvg_name = $"Test {a}",
                    User_Tvg_chno = count,
                    Tvg_chno = count,
                };


                context.VideoStreams.Add(videoStream);
                context.StreamGroupVideoStreams.Add(new StreamGroupVideoStream { StreamGroupId = 4, ChildVideoStreamId = videoStream.Id });
            }

            context.SaveChanges();
        }
    }
}
