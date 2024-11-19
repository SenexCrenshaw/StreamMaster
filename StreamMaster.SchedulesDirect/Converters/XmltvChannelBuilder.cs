using StreamMaster.Domain.Enums;

namespace StreamMaster.SchedulesDirect.Converters;

public class XmltvChannelBuilder(ILogoService logoService, ISchedulesDirectDataService schedulesDirectDataService, IOptionsMonitor<SDSettings> sdSettingsMonitor)
{
    public XmltvChannel BuildXmltvChannel(XmltvChannel xmltvChannel, VideoStreamConfig videoStreamConfig, OutputProfileDto outputProfile, string baseUrl)
    {
        XmltvChannel channel = xmltvChannel.DeepCopy();
        channel.Id = GetChannelId(videoStreamConfig, outputProfile);
        if (channel.Icons?.Count > 0)
        {
            foreach (XmltvIcon icon in channel.Icons)
            {
                icon.Src = logoService.GetLogoUrl(icon.Src, baseUrl);
            }
        }

        //XmltvChannel channel = new()
        // {
        //     Id = id,
        //     DisplayNames = []
        // };

        // string displayName = videoStreamConfig.Name;
        // channel.DisplayNames.Add(new XmltvText { Text = displayName });

        // Add additional display names if necessary
        //if (!string.IsNullOrEmpty(service.CallSign) && !service.CallSign.Equals(displayName))
        //{
        //    channel.DisplayNames.Add(new XmltvText { Text = service.CallSign });
        //}

        // Add channel numbers if requested
        //if (sdSettings.XmltvIncludeChannelNumbers)
        //{
        //    List<string> numbers = GetChannelNumbers(service);
        //    foreach (string num in numbers)
        //    {
        //        if (!channel.DisplayNames.Any(dn => dn.Text == num))
        //        {
        //            channel.DisplayNames.Add(new XmltvText { Text = num });
        //        }
        //    }
        //}

        // Add affiliate if present
        //string? affiliate = service.mxfAffiliate?.Name;
        //if (!string.IsNullOrEmpty(affiliate) && !string.IsNullOrEmpty(service.Name) && !service.Name.Equals(affiliate))
        //{
        //    channel.DisplayNames.Add(new XmltvText { Text = affiliate });
        //}

        //// Add logo if available
        //if (service.extras.TryGetValue("logo", out dynamic? logoObj))
        //{
        //    if (logoObj is StationImage stationImage)
        //    {
        //        channel.Icons =
        //        [
        //            new XmltvIcon
        //            {
        //            //{/service.EPGNumber
        //                Src = logoService.GetLogoUrl( stationImage.Url, baseUrl),
        //                Height = stationImage.Height,
        //                Width = stationImage.Width
        //            }
        //        ];
        //    }
        //}

        return channel;
    }

    public XmltvChannel BuildXmltvChannel(MxfService service, string baseUrl, bool isOG)
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
        if (service.extras.TryGetValue("logo", out dynamic? logoObj))
        {
            if (logoObj is StationImage stationImage)
            {
                channel.Icons =
                [
                    new XmltvIcon
                    {
                    //{/service.EPGNumber
                        Src = isOG ? stationImage.Url:  logoService.GetLogoUrl( stationImage.Url, baseUrl),
                        Height = stationImage.Height,
                        Width = stationImage.Width
                    }
                ];
            }
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
    public XmltvChannel BuildXmltvChannel(MxfService service, VideoStreamConfig? videoStreamConfig, OutputProfileDto outputProfile, string baseUrl)
    {
        SDSettings sdSettings = sdSettingsMonitor.CurrentValue;
        string id = GetChannelId(service, videoStreamConfig, outputProfile);

        XmltvChannel channel = new()
        {
            Id = id,
            DisplayNames = []
        };

        string displayName = service.Name ?? service.CallSign;
        channel.DisplayNames.Add(new XmltvText { Text = displayName });

        // Add additional display names if necessary
        if (!string.IsNullOrEmpty(service.CallSign) && !service.CallSign.Equals(displayName))
        {
            channel.DisplayNames.Add(new XmltvText { Text = service.CallSign });
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

        // Add logo if available
        if (service.extras.TryGetValue("logo", out dynamic? logoObj))
        {
            if (logoObj is StationImage stationImage)
            {
                channel.Icons =
                [
                    new XmltvIcon
                    {
                    //{/service.EPGNumber
                        Src = logoService.GetLogoUrl( stationImage.Url, baseUrl),
                        Height = stationImage.Height,
                        Width = stationImage.Width
                    }
                ];
            }
        }

        return channel;
    }

    public static string GetChannelId(VideoStreamConfig videoStreamConfig, OutputProfileDto outputProfile)
    {
        string id = videoStreamConfig.ChannelNumber.ToString();

        if (outputProfile.Id != nameof(ValidM3USetting.NotMapped))
        {
            switch (outputProfile.Id)
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

    private static string GetChannelId(MxfService service, VideoStreamConfig? videoStreamConfig, OutputProfileDto outputProfile)
    {
        string id = service.ChNo.ToString();

        if (outputProfile.Id != nameof(ValidM3USetting.NotMapped))
        {
            switch (outputProfile.Id)
            {
                case nameof(ValidM3USetting.Group):
                    if (videoStreamConfig != null && !string.IsNullOrEmpty(videoStreamConfig.Group))
                    {
                        id = videoStreamConfig.Group;
                    }
                    break;
                case nameof(ValidM3USetting.ChannelNumber):
                    id = service.ChNo.ToString();
                    break;
                case nameof(ValidM3USetting.Name):
                    id = service.Name;
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