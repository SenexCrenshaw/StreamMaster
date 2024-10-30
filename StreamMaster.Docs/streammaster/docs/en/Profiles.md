# Profile Configuration in StreamMaster

StreamMaster provides two primary types of profiles for managing the behavior and output of Stream Groups (**SGs**): **Output Profiles** and **Command Profiles**. These profiles allow fine-grained control over how streams are formatted and accessed, giving flexibility to accommodate different streaming needs and user environments.

## Output Profile 📜

An **Output Profile** in StreamMaster determines how the **M3U/XML files** for a Stream Group are generated and configured. This profile customizes the output format and controls which metadata (such as icons, channel numbers, and group titles) is included, creating a tailored playlist experience for users.

### Default Output Profile Settings

The default Output Profile provided by StreamMaster includes the following configuration:

| Setting                 | Description                                                                                 |
| ----------------------- | ------------------------------------------------------------------------------------------- |
| **EnableIcon**          | Shows the channel icon in the M3U/XML output, enhancing the visual display of channels.     |
| **EnableGroupTitle**    | Adds a group title for channels, which helps organize streams by group names in the output. |
| **EnableChannelNumber** | Assigns a channel number for each stream, enabling organized channel navigation.            |
| **Id**                  | Uses the channel number as a unique identifier for each stream.                             |
| **Name**                | Sets the name field for the output, usually reflecting the original stream name.            |
| **Group**               | Defines the group to which the stream belongs, displayed as a category in the playlist.     |

> **Note**: The default Output Profile is marked as **read-only** to ensure consistency. However, you can create custom profiles for specific requirements if needed.

### Customizing Output Profiles

To create a custom Output Profile:

1. Navigate to **Settings** > **Output Profiles** in the StreamMaster UI.
2. Define the metadata fields (e.g., EnableIcon, EnableChannelNumber) to customize your output file format.
3. Save the profile, and apply it to the desired Stream Group(s).

## Command Profile 🎛️

A **Command Profile** in StreamMaster specifies the streaming command used to retrieve or process streams for a Stream Group. This profile determines the method and parameters for accessing or relaying the streams, allowing for different approaches based on network setup or device requirements.

### Default Command Profiles

StreamMaster includes several default Command Profiles to handle various streaming configurations:

| Profile Name      | Command      | Description                                                                                                   |
| ----------------- | ------------ | ------------------------------------------------------------------------------------------------------------- |
| **Default**       | STREAMMASTER | Internal binary mechanism that just moves the bytes around. No processing happens                             |
| **SMFFMPEG**      | ffmpeg       | Uses `ffmpeg` with options for user agent, reconnect settings, and optimized buffering for network streaming. |
| **SMFFMPEGLocal** | ffmpeg       | Local streaming with `ffmpeg`, using reduced buffering for lower latency.                                     |
| **YT**            | yt.sh        | Runs `yt.sh` script, primarily for streaming from YouTube links.                                              |
| **Redirect**      | None         | Redirects directly to the original stream without modification.                                               |

Each profile is **read-only** by default, ensuring stable and reliable configurations.

### Key Command Profile Parameters

- **Command**: Specifies the executable command (e.g., `ffmpeg`, `yt.sh`) used to process the stream.
- **Parameters**: Additional parameters passed to the command, such as `-fflags +genpts+discardcorrupt` for error handling or `{streamUrl}` to dynamically insert the stream’s URL.

### Command Profile Path Lookup

StreamMaster automatically searches for command executables in the following directories:

- `/config` - Mapped in your docker compose
- `/usr/local/bin`
- `/usr/bin`
- `/bin`

This ensures that StreamMaster can locate and use common streaming utilities (e.g., `ffmpeg` or custom scripts like `yt.sh`) if they are installed in standard system paths.

### Customizing Command Profiles

To create a custom Command Profile:

1. Go to **Settings** > **Command Profiles** in the StreamMaster UI.
2. Select the command type and customize the parameters to suit your streaming environment.
3. Save and apply this profile to specific Stream Groups as required.

> **Tip**: Refer to the StreamMaster documentation on [Command Profiles](command_profiles.md) for advanced usage scenarios.

---

{% include-markdown "../includes/_footer.md" %}
