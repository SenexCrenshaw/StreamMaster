# 🎬 M3U Output Attributes Documentation

This document provides a detailed description of each attribute available for M3U output customization in StreamMaster. Understanding these attributes allows you to control the metadata included in your M3U playlist files, enhancing the viewing experience and organization.

---

## 🎬 M3U Output: Understanding `group-title`

The `group-title` attribute in an M3U output file is used to categorize channels into specific groups. This attribute is especially useful for organizing a large number of channels, as it enables users to filter and browse by category within supported media players and IPTV applications.

### What Does `group-title` Do?

- **Organizes Channels**: Channels with the same `group-title` are grouped together, making it easier to manage playlists.
- **Improves Navigation**: Users can browse by categories (e.g., "Sports," "News," "Movies") rather than scrolling through a long list of channels.
- **Enhances User Experience**: Assigning descriptive `group-title` labels helps users quickly locate their praeferred content, resulting in a more organized and efficient viewing experience.

### Example of `group-title` in an M3U Entry

```m3u
#EXTINF:-1 tvg-id="123" tvg-name="Channel Name" tvg-logo="http://example.com/logo.png" group-title="Sports", Channel Name
http://stream-url.com/stream
```

---

## 🖼️ M3U Output: Understanding `tvg-logo`

The `tvg-logo` attribute in an M3U output file displays a logo for each channel, providing a visual element that makes it easier to identify channels.

### What Does `tvg-logo` Do?

- **Enhances Channel Recognition**: By including a channel logo, users can quickly recognize channels by their branding.
- **Adds Visual Appeal**: Channel logos make the playlist visually appealing and organized.

### Example of `tvg-logo` in an M3U Entry

```m3u
#EXTINF:-1 tvg-id="123" tvg-name="Channel Name" tvg-logo="http://example.com/logo.png", Channel Name
http://stream-url.com/stream
```

---

## 🆔 EPG/M3U Output: Understanding `CUID` / `channel-id`

The `CUID` or `channel-id` attribute serves as a unique identifier for each channel within the M3U file. This identifier helps distinguish channels, especially in systems where multiple channels may have similar names.

### What Does `CUID` / `channel-id` Do?

- **Provides Unique Channel Identification**: Assigns a unique identifier to each channel, ensuring consistent recognition across devices and applications.
- **Enhances Integration with EPGs**: This also sets the `channel-id` in the EPG output, linking M3U channel entries with Electronic Program Guide (EPG) data for accurate scheduling.

### Example of `CUID` in an M3U Entry

```m3u
#EXTINF:-1 CUID="12345" channel-id="ChannelID" tvg-name="Channel Name" tvg-logo="http://example.com/logo.png", Channel Name
http://stream-url.com/stream
```

### Corresponding EPG XML Entry

Below is an example of how an EPG XML entry might look with the `channel-id` set to match the M3U channel entry:

```xml
<tv>
  <channel id="ChannelID">
    <display-name>Channel Name</display-name>
    <icon src="http://example.com/logo.png" />
  </channel>
  <programme start="20240101060000 +0000" stop="20240101070000 +0000" channel="ChannelID">
    <title>Morning News</title>
    <desc>The latest news updates and reports.</desc>
  </programme>
</tv>
```

---

## 📺 M3U Output: Understanding `tvg-name`

The `tvg-name` attribute allows you to set the display name for each channel, usually matching the original name of the stream or TV station.

### What Does `tvg-name` Do?

- **Displays Channel Name**: Provides a clear, identifiable name for each channel, enhancing user experience.
- **Matches Original Stream Name**: Typically reflects the name as provided by the source, ensuring consistency.

### Example of `tvg-name` in an M3U Entry

```m3u
#EXTINF:-1 tvg-id="123" tvg-name="Channel Name" tvg-logo="http://example.com/logo.png", Channel Name
http://stream-url.com/stream
```

---

## 📂 M3U Output: Understanding `tvg-group`

The `tvg-group` attribute defines the group or category to which a channel belongs. This grouping can be used in conjunction with `group-title` to enhance organization within playlists.

### What Does `tvg-group` Do?

- **Categorizes Channels**: Channels with the same `tvg-group` are grouped together, improving playlist organization.
- **Enables Easier Filtering**: Users can filter by category, similar to `group-title` functionality.

### Example of `tvg-group` in an M3U Entry

```m3u
#EXTINF:-1 tvg-id="123" tvg-name="Channel Name" tvg-group="News", Channel Name
http://stream-url.com/stream
```

---

{% include-markdown "../includes/_footer.md" %}
