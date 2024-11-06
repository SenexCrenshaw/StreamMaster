#!/bin/bash

# Enable debugging mode and log to a file
exec >/path/to/your/logfile.log 2>&1
set -x

# Get the URL parameter
url="$1"

# Log the provided URL to verify it is correct
echo "Provided URL: $url"

# Use yt-dlp to download the video and pass it to ffmpeg
/usr/local/bin/yt-dlp -q --no-warnings --ffmpeg-location /usr/local/bin/ffmpeg -f bestvideo+bestaudio -o - "$url" | /usr/local/bin/ffmpeg -i pipe:0 -c:v libx264 -c:a aac -f mpegts pipe:1
