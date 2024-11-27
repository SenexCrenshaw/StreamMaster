using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect.Converters;

public class XmltvChannelBuilder(ISchedulesDirectDataService schedulesDirectDataService, IOptionsMonitor<SDSettings> sdSettingsMonitor) : IXmltvChannelBuilder
{
    public XmltvChannel BuildXmltvChannel(XmltvChannel xmltvChannel, VideoStreamConfig videoStreamConfig)
    {
        XmltvChannel channel = xmltvChannel.DeepCopy();
        channel.Id = videoStreamConfig.OutputProfile!.Id;
        if (channel.Icons?.Count > 0)
        {
            foreach (XmltvIcon icon in channel.Icons)
            {
                icon.Src = icon.Src;
            }
        }

        return channel;
    }

    public XmltvChannel BuildXmltvChannel(MxfService service, bool isOG)
    {
        SDSettings sdSettings = sdSettingsMonitor.CurrentValue;

        XmltvChannel channel = new()
        {
            Id = service.StationId,
            DisplayNames = []
        };

        string displayName = service.Name ?? service.CallSign;
        channel.DisplayNames.Add(new XmltvText { Text = displayName });

        // Add additional display names if necessary
        if (!string.IsNullOrEmpty(service.CallSign) && !service.CallSign.Equals(displayName))
        {
            channel.DisplayNames.Add(new XmltvText { Text = service.CallSign });
        }

        // Add logo if available
        if (service.XmltvIcon != null)
        {
            channel.Icons = [service.XmltvIcon];
        }

        if (isOG)
        {
            return channel;
        }

        // Add channel numbers if requested
        if (sdSettings.XmltvIncludeChannelNumbers)
        {
            List<string> numbers = GetChannelNumbers(service);
            foreach (string num in numbers)
            {
                if (!channel.DisplayNames.Any(dn => dn.Text == num))
                {
                    channel.DisplayNames.Add(new XmltvText { Text = num });
                }
            }
        }

        // Add affiliate if present
        string? affiliate = service.mxfAffiliate?.Name;
        if (!string.IsNullOrEmpty(affiliate) && !string.IsNullOrEmpty(service.Name) && !service.Name.Equals(affiliate))
        {
            channel.DisplayNames.Add(new XmltvText { Text = affiliate });
        }

        return channel;
    }
    public static string GetChannelId(VideoStreamConfig videoStreamConfig)
    {
        string id = videoStreamConfig.ChannelNumber.ToString();
        if (videoStreamConfig.OutputProfile is null)
        {
            return id;
        }
        if (videoStreamConfig.OutputProfile.Id != nameof(ValidM3USetting.NotMapped))
        {
            switch (videoStreamConfig.OutputProfile.Id)
            {
                case nameof(ValidM3USetting.Group):
                    if (videoStreamConfig != null && !string.IsNullOrEmpty(videoStreamConfig.Group))
                    {
                        id = videoStreamConfig.Group;
                    }
                    break;
                case nameof(ValidM3USetting.ChannelNumber):
                    id = videoStreamConfig.ChannelNumber.ToString();
                    break;
                case nameof(ValidM3USetting.Name):
                    id = videoStreamConfig.Name;
                    break;
            }
        }

        return id;
    }

    private List<string> GetChannelNumbers(MxfService service)
    {
        List<string> numbers = [];

        foreach (MxfLineup mxfLineup in schedulesDirectDataService.AllLineups)
        {
            foreach (MxfChannel mxfChannel in mxfLineup.Channels)
            {
                if (mxfChannel.Service != service.Id || mxfChannel.Number <= 0)
                {
                    continue;
                }

                string num = $"{mxfChannel.Number}" + (mxfChannel.SubNumber > 0 ? $".{mxfChannel.SubNumber}" : "");

                if (!numbers.Contains(num))
                {
                    numbers.Add(num);
                }
            }
        }

        return numbers;
    }
}