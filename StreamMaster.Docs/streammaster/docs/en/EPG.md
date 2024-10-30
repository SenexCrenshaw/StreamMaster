# EPG/XML Files

## What is an EPG/XML File? 📘

An **EPG (Electronic Program Guide)** file, often formatted in XML (specifically XMLTV format), is used to provide program guide information for streaming channels. EPG files list details about scheduled programs, such as titles, start and end times, descriptions, and genres. This guide data enhances the viewing experience by allowing users to browse current and upcoming shows in a structured format.

In **StreamMaster (SM)**, EPG/XML files can be imported to integrate program guides with your IPTV channels, enriching the user experience with program schedules and metadata.

## Importing EPG/XML Files in StreamMaster 🛠

To add EPG/XML files to StreamMaster, use the **Import EPG** option within the StreamMaster UI. StreamMaster provides several options during import to customize how the EPG data is processed and displayed. Below are the primary options available when importing an EPG file.

### EPG Import Options

| Option              | Description                                                                                 |
| ------------------- | ------------------------------------------------------------------------------------------- |
| **Name**            | The name you want to assign to the imported EPG file.                                       |
| **File Name**       | The local name to save the file under once imported.                                        |
| **EPG Number**      | A unique identifier for the EPG file, allowing differentiation among multiple EPG sources.  |
| **Time Shift**      | (Optional) Adjusts the time of all programs by the specified number of hours.               |
| **Hours To Update** | Interval (in hours) to automatically check and refresh the EPG file.                        |
| **Url/File Source** | The URL or local file path of the EPG file to be imported.                                  |
| **Color**           | (Optional) Assigns a color to the guide entries for easier identification in the interface. |

### Import Process Overview

Once an EPG/XML file is added using these options, StreamMaster:

1. **Validates** - Ensures the provided URL or local file source is accessible.
2. **Fetches and Parses Content** - Downloads and reads the XML content to populate guide information.
3. **Processes EPG Data** - Adjusts time zones, applies colors, and links programs to channels as per user settings.
4. **Saves and Updates** - The EPG file is saved within StreamMaster’s repository, updating program data periodically as specified.

### Error Handling 🔄

During the import, StreamMaster performs several checks to verify the EPG file:

- If the **URL source** is invalid or inaccessible, an error message is displayed, and the file is not added.
- If the **file format** is unsupported or unreadable, StreamMaster stops further processing and alerts the user.
- **Automatic cleanup** occurs on failure to ensure no incomplete files are left in the system.

### Troubleshooting Import Issues

If an import fails, check the following:

- Verify the **URL or file path** is correct and accessible.
- Confirm the **EPG Number** is unique and not in use by other EPG files.
- Ensure **required fields** (such as Name and UrlSource) are filled out.

### Automating EPG File Refresh 🚀

StreamMaster can automatically update and refresh EPG files. Set the `HoursToUpdate` option to specify the frequency of refresh, keeping your guide data current without manual intervention.

---

{%
    include-markdown "../includes/_footer.md"
%}
