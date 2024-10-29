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
      - 7096:7096 # Only for SSL
    environment:
      - PUID=1000 # UID to Run As
      - PGID=1000 # GID to Run As
    restart: unless-stopped
    volumes:
      - ~/.streammaster:/config
      - ~/.streammaster/tv-logos:/config/tv-logos
```

### Configuration Explained 🔍

- **Image**: Specifies the StreamMaster image to use.
- **Ports**:
  - `7095`: The default port for StreamMaster’s web interface.
  - `7096`: Used for SSL connections (optional).
- **Environment Variables**:
  - `PUID` and `PGID`: Set the user and group IDs that the container will use, providing file permission control.
- **Volumes**:
  - `~/.streammaster:/config`: Stores your configuration data.
  - `~/.streammaster/tv-logos:/config/tv-logos`: Stores custom TV logos.

> **Tip**: Adjust the file paths if you prefer a different directory for your configuration files.

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
