# 🔐 Authentication

Stream Master supports two primary authentication methods, ensuring secure access for both the User Interface (UI) and generated links. Here’s an overview of each:

## 1. UI Authentication (Forms/None)

UI authentication controls access to Stream Master’s user interface. There are two options for UI auth, **Forms** and **None**.

### 1.1 Forms Authentication

Using Forms-based authentication requires users to log in with a username and password, enhancing security by limiting access to verified users.

#### Setup

1. In the Stream Master web UI, navigate to the **Settings** section.
2. Under **Authentication**, select `Forms` as the authentication method.
3. Define user credentials directly within the web UI.

### 1.2 None (No Authentication)

Selecting **None** disables UI authentication, allowing unrestricted access to the UI. This option is suitable for environments where access is controlled at the network level or the instance is used in a secure, private setting.

#### Setup

1. In the Stream Master web UI, navigate to **Settings** > **Authentication**.
2. Set the authentication method to `None`.

> **⚠️ Note**: Choosing **None** leaves the UI accessible to anyone with network access to the server. Only use this option in trusted and controlled environments.

---

## 2. Encrypted Link Authentication

Stream Master generates HD Home Run (HDHR), XML, and M3U links, each of which can be encrypted based on a **Stream Group (SG) group key**. This key-based encryption provides secure links while allowing a simpler, unencrypted link for easy local access.

### 2.1 SG Group Key for Link Encryption

Each **Stream Group (SG)** in Stream Master has a **group key** used to encrypt links. You can manage this key within the **Streams** > **SG** section of the web UI. The encryption secures external access while maintaining a straightforward local option.

#### Link Types

- **Encrypted Links**: Secure HDHR, XML, and M3U links generated based on the SG group key.
- **Short Links**: Unencrypted, user-friendly links intended for local access.

#### Setup

1. In the Stream Master web UI, navigate to **Streams** > **SG (Stream Group)**.
2. Enter a unique, strong group key to encrypt the links generated for the group.

#### Usage

- Once the group key is set, encrypted HDHR, XML, and M3U links are automatically generated using this key.
- To reset or update the encryption, change the group key. Note that this will require updating any previously shared encrypted links.

---

## 🔄 Changing Authentication and Link Settings

To update authentication or link encryption settings, use the Stream Master web UI in the relevant sections. Changes are applied immediately without needing manual configuration file edits.

---

{%
    include-markdown "../includes/_footer.md"
%}
