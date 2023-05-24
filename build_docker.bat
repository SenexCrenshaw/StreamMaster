@echo off
for /f "tokens=1 delims=-" %%a in ("%1") do set VERSION=%%a
docker buildx build . --push --platform linux/amd64,linux/arm64 --tag senexcrenshaw/streammaster:latest --tag senexcrenshaw/streammaster:%VERSION% --tag senexcrenshaw/streammaster:%1
