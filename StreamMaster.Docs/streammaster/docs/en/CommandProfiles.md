# 📘 Command Profiles

Command Profiles in StreamMaster specify the streaming commands used to retrieve or process streams for a **Stream Group**. These profiles determine the method and parameters for accessing or relaying streams, enabling different approaches based on network setup, device requirements, or performance preferences.

---

## Default Command Profiles 🚀

StreamMaster includes several default Command Profiles, each optimized for common streaming scenarios. Below is a summary of each profile:

> **⚠️ Defaults Notice:** _Cannot be edited._ To modify settings, **create a new profile** and navigate to `Settings` to change the system default.

| **Profile Name**  | **Command**  | **Description**                                                                                                    |
| ----------------- | ------------ | ------------------------------------------------------------------------------------------------------------------ |
| **Default**       | STREAMMASTER | Internal binary mechanism that moves the bytes without processing.                                                 |
| **SMFFMPEG**      | `ffmpeg`     | Uses ffmpeg with options for user agent, reconnect settings, and optimized buffering for stable network streaming. |
| **SMFFMPEGLocal** | `ffmpeg`     | Optimized for local streaming with ffmpeg, utilizing reduced buffering to achieve lower latency.                   |
| **YT**            | `yt.sh`      | Executes the `yt.sh` script, primarily designed for streaming from YouTube links.                                  |
| **Redirect**      | None         | Directly redirects to the original stream without any modification.                                                |

> **Note:** Each default Command Profile is read-only by default, providing stable and reliable configurations to ensure a consistent experience.

---

## Parameter Substitutions 📝

StreamMaster provides two useful parameter substitutions to simplify dynamic configurations:

- **`{clientUserAgent}`**: Inserts the client’s user agent string, allowing streams to be configured for specific devices or browsers.
- **`{streamUrl}`**: Dynamically replaces with the URL of the stream being accessed.

These substitutions let you customize commands without hardcoding specific values, making your Command Profiles more versatile.

---

## Example Command Profiles and Parameters 🔧

Below are examples demonstrating how Command and Parameters are set within profiles, and how StreamMaster substitutes values dynamically:

### Example 1: SMFFMPEG Profile

- **Command**: `ffmpeg`
- **Parameters**: `-user_agent "{clientUserAgent}" -reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 2 -i "{streamUrl}" -f mpegts -fflags +genpts+discardcorrupt`

**Resulting Command with Substitutions:**
If the `{clientUserAgent}` is set to `Mozilla/5.0 (Windows NT 10.0; Win64; x64)` and `{streamUrl}` is `http://example.com/live-stream`, StreamMaster will generate:

```bash
ffmpeg -user_agent "Mozilla/5.0 (Windows NT 10.0; Win64; x64)" -reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 2 -i "http://example.com/live-stream" -f mpegts -fflags +genpts+discardcorrupt
```

### Example 2: YT Profile

- **Command**: `yt.sh`
- **Parameters**: `"{streamUrl}"`

**Resulting Command with Substitution:**
If `{streamUrl}` is set to a YouTube link such as `https://www.youtube.com/watch?v=dQw4w9WgXcQ`, StreamMaster will generate:

```bash
yt.sh "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
```

---

## Command Profile Path Lookup 🗂️

StreamMaster automatically searches for command executables within the following directories:

- `/config` — Configured within your Docker Compose setup.
- `/usr/local/bin`
- `/usr/bin`
- `/bin`

This allows StreamMaster to locate and use common streaming utilities (e.g., `ffmpeg` or custom scripts like `yt.sh`), provided they’re installed in standard system paths.

---

## Customizing Command Profiles 🛠️

To create or edit a custom Command Profile:

1. Go to **Streams > ![Command Profiles Overview](assets/profiles.png) > Command Profiles** in the StreamMaster UI.
2. Create or edit a command profile and configure its parameters to suit your streaming requirements.
3. Save and apply this profile to the desired Stream Groups.

> **Note:** System default profiles cannot be modified to ensure stability and reliability in default configurations.

> **Tip:** For advanced scenarios, consult the [Discord channel on Command Profiles](https://discord.com/channels/1075403862124531753/1296815673472974878) for examples and in-depth usage.

By using Command Profiles effectively, you can optimize StreamMaster for diverse network configurations and streaming setups. Explore the available profiles and customize as needed to achieve the best results!

---

{% include-markdown "../includes/_footer.md" %}
