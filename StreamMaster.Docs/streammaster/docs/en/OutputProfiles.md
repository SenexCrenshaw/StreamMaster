# 📘 Output Profiles

An **Output Profile** in StreamMaster determines how the **M3U/XML files** for a Stream Group are generated and configured. This profile customizes the output format and controls which metadata (such as icons, channel numbers, and group titles) is included, creating a tailored playlist experience for users.

---

## Default Output Profile Settings 🛠️

The default Output Profile provided by StreamMaster includes the following configuration settings:

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
