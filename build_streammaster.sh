#!/bin/bash
BUILD_DIR="$HOME/src"
apt update && apt -y upgrade

apt-get install -y git curl
apt-get install -y dotnet-sdk-7.0
curl -sL https://deb.nodesource.com/setup_18.x | bash - && apt-get install -yq nodejs build-essential

mkdir -p "$BUILD_DIR/publish"

cd "$BUILD_DIR"
git clone https://github.com/SenexCrenshaw/StreamMaster.git
cd StreamMaster
git fetch --tags
latest_tag=$(git describe --tags $(git rev-list --tags --max-count=1))
git checkout $latest_tag

dotnet restore "StreamMasterAPI/StreamMasterAPI.csproj"
cd StreamMasterAPI
dotnet build "StreamMasterAPI.csproj" -c Debug >/dev/null
cd ..

cd streammasterwebui
npm install
npm run build
cp -R build/* "$BUILD_DIR/publish/wwwroot/"
cd ..

cd StreamMasterAPI
dotnet publish --no-restore "StreamMasterAPI.csproj" -c Debug -o "$BUILD_DIR/publish" /p:UseAppHost=false >/dev/null

echo "Stream Master build here \"$BUILD_DIR/publish/\""
echo "export ASPNETCORE_URLS=\"http://+:7095\""
echo "cd \"$BUILD_DIR/publish/\""
echo "dotnet StreamMasterAPI.dll"