# 📤 Output Profiles

An **Output Profile** in StreamMaster determines how the **M3U/XML files** for a Stream Group are generated and configured. This profile customizes the output format and controls which metadata (such as icons, channel numbers, and group titles) is included, creating a tailored playlist experience for users.

---

## Default Output Profile Settings 🛠️

The default Output Profile provided by StreamMaster includes the following configuration settings:

> **⚠️ Defaults Notice:** _Cannot be edited._ To modify settings, **create a new profile** and navigate to `Settings` to change the system default.

| **Setting**               | **Description**                                          |
| ------------------------- | -------------------------------------------------------- |
| **Enable Icon**           | Enables `tvg-logo`.                                      |
| **Enable Group Title**    | Enables `group-title`.                                   |
| **Enable Channel Number** | Enables `tvg-chno` / `channel-number`                    |
| **Id**                    | Maps what is used for the ID field `CUID` / `channel-id` |
| **Name**                  | Maps what is used for the Name field `tvg-name`          |
| **Group**                 | Maps what is used for the Group field `tvg-group`        |

> **Note:** The default Output Profile is **read-only** and cannot be deleted or modified to ensure consistency. However, you can create custom profiles to meet specific output requirements.

---

## Mapping Options for `Id`, `Name`, and `Group` Settings

The `Id`, `Name`, and `Group` settings within an Output Profile can be mapped to various metadata fields, allowing for greater flexibility in how channel information is displayed. Each setting can be mapped to one of the following:

- **Not Mapped**: Leaves the field unmapped, meaning it won’t be included in the output.
- **Name**: Maps to the name of the channel.
- **Group**: Maps to the group name.
- **Channel Number**: Maps to a specific channel number.
- **Channel Name**: Maps to the original channel name.

These mapping options allow you to tailor how each piece of metadata is represented in the M3U/XML output, enhancing the viewer’s experience.

---

[Understanding output attributes](M3U_Output_Attributes.md).

---

## Customizing Output Profiles 🎛️

To create or modify a custom Output Profile:

1. Go to **Settings > Output Profiles** in the StreamMaster UI (the same area as **Command Profiles**).
2. Configure metadata fields such as `EnableIcon`, `EnableChannelNumber`, and others to customize the output format.
3. Save your custom profile, then apply it to the desired Stream Groups.

This flexibility allows you to organize streams with the metadata and display options best suited to your setup.

> **Tip:** Custom profiles enable a more organized and personalized output by controlling which information appears in the M3U/XML file. Experiment with the settings to create a user-friendly layout!

By using Output Profiles effectively, you can create a more engaging and accessible playlist experience tailored to your audience's needs.

---

{% include-markdown "../includes/_footer.md" %}
