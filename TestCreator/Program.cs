using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Models;
using StreamMaster.Domain.Services;
using StreamMaster.Infrastructure.EF;
using StreamMaster.Infrastructure.Services;
using StreamMaster.Infrastructure.Services.Settings;
using StreamMaster.SchedulesDirect;
using StreamMaster.SchedulesDirect.Converters;
using StreamMaster.SchedulesDirect.Data;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Helpers;

using System.Reflection;

namespace TestCreator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddLogging(configure => configure.AddConsole())
                .AddSingleton<ISchedulesDirectDataService, SchedulesDirectDataService>()
                .AddTransient<IXmltv2Mxf, XmlTv2Mxf>()
                .AddSingleton<IMemoryCache, MemoryCache>()
                .AddSingleton<IEPGHelper, EPGHelper>()
                .AddSingleton<ISettingsService, SettingsService>()
                .AddSingleton<IIconService, IconService>()
                .AddSingleton<IXMLTVBuilder, XMLTVBuilder>()
                .AddInfrastructureEFServices()
                .AddAutoMapper(
                    Assembly.Load("StreamMaster.Domain"),
                    Assembly.Load("StreamMaster.Application"),
                    Assembly.Load("StreamMaster.Infrastructure"),
                    Assembly.Load("StreamMaster.Streams")
                )
                .BuildServiceProvider();

            RepositoryContext? wrapper = serviceProvider.GetService<RepositoryContext>();
            StreamGroup sg = wrapper.StreamGroups.First(a => a.Id == 4);

            IQueryable<string> testStreams = wrapper.VideoStreams.Where(a => a.User_Tvg_group == "LOCAL CHANNELS - USA").Select(a => a.Id);
            List<string> childStreams = wrapper.StreamGroupVideoStreams.Where(a => a.StreamGroupId == 4).Select(a => a.ChildVideoStreamId).ToList();

            IQueryable<string> missing = testStreams.Except(childStreams);

            Console.WriteLine($"Total test streams {testStreams.Count()}");
            Console.WriteLine($"Total test streams to update {missing.Count()}");

            foreach (string miss in missing)
            {
                wrapper.StreamGroupVideoStreams.Add(new StreamGroupVideoStream { StreamGroupId = 4, ChildVideoStreamId = miss });
            }
            wrapper.SaveChanges();
        }
    }
}
