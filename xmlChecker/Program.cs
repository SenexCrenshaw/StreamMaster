using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Logging;
using StreamMaster.Domain.Services;
using StreamMaster.Infrastructure.Services.Settings;
using StreamMaster.SchedulesDirect;
using StreamMaster.SchedulesDirect.Converters;
using StreamMaster.SchedulesDirect.Data;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.XmltvXml;
using StreamMaster.SchedulesDirect.Helpers;

using System.Diagnostics;
namespace xmlChecker;

internal class Program
{
    private static int fileIndex = 1000;
    private static ILogger<Program> _logger;
    private static IXmltv2Mxf _xmltv2Mxf;
    private static readonly string dir = @"C:\Users\senex\git\StreamMaster\xmlChecker\bin\Debug\net8.0\";
    private static void Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


        ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging(configure =>
            {
                configure.AddConsole();
                configure.AddConfiguration(config.GetSection("Logging"));

            })
            .AddSingleton<ISchedulesDirectDataService, SchedulesDirectDataService>()
            .AddSingleton<IXmltv2Mxf, XmlTv2Mxf>()
            .AddSingleton<IMemoryCache, MemoryCache>()
            .AddSingleton<IEPGHelper, EPGHelper>()
            .AddSingleton<ISettingsService, SettingsService>()
            .AddSingleton<IXMLTVBuilder, XMLTVBuilder>()
            .BuildServiceProvider();

        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        GlobalLoggerProvider.Configure(loggerFactory);

        _logger = serviceProvider.GetService<ILogger<Program>>()!;

        _xmltv2Mxf = serviceProvider.GetRequiredService<IXmltv2Mxf>();

        SchedulesDirectData.WriteCSV = true;

        Read(@"C:\Users\senex\git\test\epg123.xmltv");

        Read(@"C:\Users\senex\git\test\epg.xml");

        Console.WriteLine("Hello, World!");
    }

    private static void Read(string file)
    {
        if (File.Exists(dir + SchedulesDirectData.serviceCSV))
        {
            File.Delete(dir + SchedulesDirectData.serviceCSV);
        }

        if (File.Exists(dir + SchedulesDirectData.programsCSV))
        {
            File.Delete(dir + SchedulesDirectData.programsCSV);
        }

        Stopwatch sw = Stopwatch.StartNew();
        _logger.LogInformation($"Reading \"{file}\"");
        XMLTV? tv = _xmltv2Mxf.ConvertToMxf(file, fileIndex);
        sw.Stop();
        if (tv == null)
        {
            _logger.LogError($"Read failed in {sw.ElapsedMilliseconds} ms");
        }
        else
        {
            _logger.LogInformation($"Read channels: {tv.Channels.Count} programs: {tv.Programs.Count} in {sw.ElapsedMilliseconds} ms");
            if (File.Exists(dir + SchedulesDirectData.serviceCSV))
            {
                string destFile = Path.GetFileNameWithoutExtension(file) + ".service.csv";
                File.Move(dir + SchedulesDirectData.serviceCSV, dir + destFile, true);
            }

            if (File.Exists(dir + SchedulesDirectData.programsCSV))
            {
                string destFile = Path.GetFileNameWithoutExtension(file) + ".programs.csv";
                File.Move(dir + SchedulesDirectData.programsCSV, dir + destFile, true);
            }
        }
        ++fileIndex;
    }
}
