#!/bin/bash
url="$1"
/usr/local/bin/yt-dlp -q --no-warnings --ffmpeg-location /usr/local/bin/ffmpeg -f bestvideo+bestaudio -o - "$url" | /usr/local/bin/ffmpeg -i pipe:0 -c:v libx264 -c:a aac -f mpegts pipe:1
