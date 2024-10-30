# Stream Groups

## What is a Stream Group? 📘

A **Stream Group** in StreamMaster is a way to organize and manage multiple streaming channels under a single grouping. Stream groups allow users to apply specific profiles, commands, and access keys, creating tailored configurations that can be easily managed and applied to various streams. This feature is particularly useful for organizing channels by category or applying specific playback settings to different groups of channels.

## Creating a Stream Group 🛠

To add a new Stream Group in StreamMaster, use the **Create Stream Group** option within the StreamMaster UI. Here, you can define the group’s name and optionally specify an output profile, command profile, and a unique group key. Below are the primary options available when creating a Stream Group.

### Stream Group Options

| Option                   | Description                                                                                                       |
| ------------------------ | ----------------------------------------------------------------------------------------------------------------- |
| **Name**                 | The name of the Stream Group. This is required and must be unique.                                                |
| **Output Profile Name**  | (Optional) Specifies the output profile to apply to all streams within this group.                                |
| **Command Profile Name** | (Optional) Assigns a command profile that determines specific playback or streaming commands for the group.       |
| **Group Key**            | (Optional) A unique identifier key for the group. If not specified, StreamMaster will generate one automatically. |

### Creating and Managing Stream Groups

Once a Stream Group is created using these options, StreamMaster:

1. **Validates** - Ensures that the name is unique and not reserved. (e.g., "all" is reserved and cannot be used).
2. **Generates a Unique ID** - Uses an internal generator to assign a unique device ID for the group.
3. **Applies Profiles** - Adds default or specified profiles for output and commands, allowing customization of streaming behavior.
4. **Saves and Updates** - The Stream Group is saved within StreamMaster’s repository and becomes available for immediate use.

# 🔐 Stream Group Encryption & Security in StreamMaster

StreamMaster offers robust security options for controlling access and securing links within **Stream Groups (SGs)**. Stream Groups allow for encryption through group keys, providing a straightforward yet secure way to manage link access. This section covers the security features available for Stream Groups and explains how they work alongside StreamMaster’s broader authentication features.

## Stream Group Security Features 🔒

Stream Groups in StreamMaster can be configured with unique **Group Keys**. These keys allow for:

- **Link Encryption**: Encrypts generated HDHR, XML, and M3U links, securing access to channels within the Stream Group.
- **Custom Access Control**: Each Group Key limits access to only those with the corresponding key, adding a layer of security for external connections.

### 1. Link Encryption with Group Keys

When a Group Key is set up for a Stream Group, any links generated (e.g., HDHR, XML, and M3U) for that group are encrypted. This ensures that only users with the appropriate decryption key or authorized access can view the stream links, enhancing security for external access.

#### Setting Up Link Encryption

1. **Navigate** to the StreamMaster web UI.
2. Go to **Streams** > **Stream Group (SG)** and select the desired Stream Group.
3. **Assign a unique Group Key** to encrypt links generated for the group.

> **Note**: StreamMaster also provides **short, unencrypted links** for easier local access, ideal for secure, private network environments.

### 2. Managing Group Keys for Security

StreamMaster allows you to update or change Group Keys at any time. Changing a Group Key automatically regenerates all encrypted links associated with that Stream Group, providing a fresh layer of security.

#### Steps to Update Group Keys

1. In the StreamMaster UI, navigate to **Streams** > **Stream Group**.
2. Select the Stream Group you wish to update.
3. Enter a new Group Key and save the changes.
4. **Update any shared links** to reflect the new Group Key encryption.

> For more details on how Group Keys and authentication work, see our [Authentication Guide](Auth.md).

---

## Overall Authentication and Link Security

StreamMaster supports two main types of authentication:

- **UI Authentication** (Forms/None): Controls access to the web interface.
- **Encrypted Link Authentication**: Secures streaming links using Group Keys.

For more on these options, refer to the

### Example Use Cases for Stream Groups

- **Categorizing Channels by Genre**: Grouping sports channels, news channels, or movie channels for easier access and management.
- **Applying Custom Profiles**: Using specific output profiles for different channel groups, such as high-definition profiles for sports or low-bandwidth profiles for mobile viewing.
- **Secure Access Control**: Setting unique group keys to control access to certain channels within a group, providing flexibility in managing permissions.

### Troubleshooting Stream Group Creation Issues 🔄

If an error occurs during Stream Group creation:

- Ensure the **Name** is filled out and doesn’t use reserved words like "all".
- Check that any specified **profiles** (Output Profile or Command Profile) exist and are correctly named.
- If no Group Key is provided, StreamMaster will generate one automatically.

---

{%
    include-markdown "../includes/_footer.md"
%}
