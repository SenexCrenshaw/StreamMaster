# Getting Started with StreamMaster 🚀

Welcome to the **StreamMaster Documentation**! This guide will help you set up StreamMaster quickly and easily using Docker Compose. Follow the instructions below to get your streaming environment up and running.

## Why Use Docker Compose?

Using Docker Compose simplifies the installation and configuration process by defining your setup in a single YAML file. This approach makes it easy to manage, update, and scale StreamMaster with minimal commands.

---

## Prerequisites 🛠

Before you begin, ensure the following are installed:

1. **Docker** - Install Docker from [Docker's official website](https://docs.docker.com/get-docker/).
2. **Docker Compose** - Most Docker installations include Docker Compose, but you can verify with:

   ```bash
   docker-compose --version
   ```

---

## Supported Architectures 🖥️

StreamMaster supports multiple architectures to ensure compatibility across various systems:

- **amd64** (most desktop/server environments)
- **arm64** (suitable for ARM-based devices)

The correct image for your architecture can be automatically pulled by using the `senexcrenshaw/streammaster:latest` tag.

---

## Version Tags 🔖

StreamMaster provides the following tags for specific builds:

- `latest`: The latest stable release
- `x.x.x`: Specific release versions
- `nightly`: Nightly builds for testing latest features (may be unstable)

---

## Setting Up StreamMaster with Docker Compose

### Step 1: Create the Docker Compose File

Create a `docker-compose.yml` file in your preferred directory and copy the following configuration into it:

```yaml
services:
  streammaster:
    image: senexcrenshaw/streammaster:latest
    container_name: streammaster
    ports:
      - 7095:7095
      - 7096:7096 # Optional, for SM to host SSL
    environment:
      DEFAULT_PORT: 7095 # Default
      DEFAULT_SSL_PORT: 7096 # Default
      PUID: 1000
      PGID: 1000
      POSTGRES_USER: postgres # Default
      POSTGRES_PASSWORD: sm123 # Default
      PGDATA: /config/DB # Default
      POSTGRES_HOST: 127.0.0.1 # Default
      POSTGRES_DB: StreamMaster # Default
      BACKUP_VERSIONS_TO_KEEP: 5 # Default
      POSTGRES_USER_FILE: /var/run/secrets/postgres-user # Optional, see Secrets
      POSTGRES_PASSWORD_FILE: /var/run/secrets/postgres-password # Optional, see Secrets
    restart: unless-stopped
    secrets: # Optional, see Secrets
      - postgres-user
      - postgres-password
    volumes:
      - ~/.streammaster:/config
      - ~/.streammaster/tv-logos:/config/tv-logos

  secrets: # Optional, see Secrets
    postgres-user:
      file: postgres-user.txt
    postgres-password:
      file: postgres-password.txt
```

---

### Configuration Explained 🔍

- **Image**: Specifies the StreamMaster image to use. `senexcrenshaw/streammaster:latest`
- **Ports**:
  - `7095`: The default HTTP port for StreamMaster’s web interface (default: `7095`).
    _Make sure port matches the `DEFAULT_PORT`._
  - `7096`: The HTTPS (SSL) port (optional, default: `7096`).
    _Make sure port matches the `DEFAULT_SSL_PORT`._
- **Environment Variables**: The following environment variables are set
  - `DEFAULT_PORT`: The default HTTP port for StreamMaster’s web interface (default: `7095`).
  - `DEFAULT_SSL_PORT`: The HTTPS (SSL) port (optional, default: `7096`).
  - `PUID` and `PGID`: Set the user and group IDs that the container will use, ensuring file permissions (default: `1000`).
  - `POSTGRES_USER`: Defines the PostgreSQL database user (default: `postgres`).
  - `POSTGRES_PASSWORD`: Password for the PostgreSQL user (default: `sm123`).
  - `PGDATA`: Directory where PostgreSQL data is stored (default: `/config/DB`).
  - `POSTGRES_HOST`: The host address for the PostgreSQL server (default: `127.0.0.1`).
  - `POSTGRES_DB`: Name of the PostgreSQL database (default: `StreamMaster`).
  - `BACKUP_VERSIONS_TO_KEEP`: Number of backup versions to retain (default: `5`).
  - `POSTGRES_USER_FILE`: Path to a secret file containing the PostgreSQL user. If specified, this value will override `POSTGRES_USER`.
  - `POSTGRES_PASSWORD_FILE`: Path to a secret file containing the PostgreSQL password. If specified, this value will override `POSTGRES_PASSWORD`.
- **Volumes**:
  - `~/.streammaster:/config`: Stores configuration data.
  - `~/.streammaster/tv-logos:/config/tv-logos`: Stores custom TV logos.

> **Tip**: Adjust the file paths if you prefer a different directory for your configuration files.

---

## Secrets 🔐

You can enhance security by using Docker secrets to store sensitive data such as PostgreSQL credentials. Define secrets in `postgres-user.txt` and `postgres-password.txt` files, then use `POSTGRES_USER_FILE` and `POSTGRES_PASSWORD_FILE` in the `docker-compose.yml` to reference these files.

Example:

```yaml
secrets:
  postgres-u:
    file: ./postgres-user.txt
  postgres-p:
    file: ./postgres-password.txt
```

---

## TV Logos Directory 📺

StreamMaster can use custom logos located in a designated folder. Place any `.jpg` or `.png` files in the `tv-logos` directory, structured as needed. StreamMaster will scan this directory at startup.

For example, a logo saved as `countries/albania/abc-al.png` will appear in StreamMaster as `countries-albania-abc-al`.

---

### Step 2: Start StreamMaster

To start StreamMaster, navigate to the directory with your `docker-compose.yml` file and run:

```bash
docker-compose up -d
```

- The `-d` flag runs the service in detached mode, meaning it will run in the background.

---

{%
   include-markdown "../includes/_footer.md"
%}
