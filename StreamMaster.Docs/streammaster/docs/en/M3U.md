# M3U Files

## What is an M3U File? 📘

An **M3U file** is a text-based playlist file used widely for streaming and multimedia purposes. It contains a list of media files, typically pointing to streaming sources such as TV channels or radio stations, and is often formatted in `.m3u` or `.m3u8` extensions. M3U files are commonly used for IPTV (Internet Protocol Television) services, allowing users to load and manage channel streams seamlessly.

In **StreamMaster (SM)**, M3U files enable users to import IPTV channel listings, offering flexible options for managing and customizing their channel streams.

## Importing M3U Files in StreamMaster 🛠

To add M3U files to StreamMaster, use the **Import M3U** option within the StreamMaster UI. StreamMaster provides several customizable options during import, which allows for fine-tuning how the imported streams function and display. Below are the primary options available when importing an M3U file.

### M3U Import Options

| Option                        | Description                                                                               |
| ----------------------------- | ----------------------------------------------------------------------------------------- |
| **Name**                      | The name you want to assign to the imported M3U file.                                     |
| **Max Stream Count**          | Sets a maximum limit on the number of concurrent streams from each M3U file.              |
| **M3U8 OutPut Profile**       | Specifies the output profile if using M3U8 format for adaptive streaming options.         |
| **M3U Key**                   | A key parameter for uniquely idenitfying a stream                                         |
| **M3U Name**                  | Which field to set the name of the channel created from a stream                          |
| **Default Stream Group Name** | Sets a default group when sync is on.                                                     |
| **Url/File Source**           | The URL/Local File of the M3U file to be imported.                                        |
| **Sync Channels**             | Boolean flag to enable/disable automatic synchronization of channels from the M3U source. |
| **Hours To Update**           | Interval (in hours) to check and refresh the M3U file.                                    |
| **Starting Channel Number**   | Sets a starting number for channels, useful for custom numbering sequences.               |
| **Auto Set Channel Numbers**  | Automatically assign channel numbers to each entry from the M3U.                          |
| **VOD Tags**                  | A list of tags used for categorizing VOD (Video on Demand) content within the M3U file.   |

### Import Process Overview

Once an M3U file is added using these options, StreamMaster:

1. **Validates** - Ensures the provided URL/Local source is accessible.
2. **Fetches and Parses Content** - Downloads and reads the M3U content to generate stream entries.
3. **Syncs and Organizes Channels** - Based on the selected options, channels are organized, and metadata is applied.
4. **Saves and Updates** - The file is saved within StreamMaster’s repository and refreshed periodically based on the specified interval.

### Error Handling 🔄

During the import, StreamMaster performs several checks to verify the M3U file:

- If the **URL source** is invalid or inaccessible, an error will be displayed, and the file is not added.
- When **no streams** are detected in the M3U, StreamMaster alerts the user and stops further processing.
- **Automatic cleanup** is conducted on failure to ensure no incomplete files are left in the system.

### Troubleshooting Import Issues

If an import fails, check the following:

- Verify the **URL source** is correct and accessible.
- Ensure the **MaxStreamCount** is set within the range of available channels in the M3U.
- Confirm that **required fields** (like Name and UrlSource) are filled out.

### Automating M3U File Refresh 🚀

StreamMaster can automatically update and refresh M3U files. Set the `HoursToUpdate` option to specify the refresh frequency, keeping your streams up-to-date without manual intervention.

---

{%
   include-markdown "../includes/_footer.md"
%}
