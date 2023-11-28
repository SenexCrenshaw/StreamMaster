$env:DOCKER_BUILDKIT = 0
docker build -f ./Dockerfile -t ffmpeg-hwaccel .
$id = $(docker create ffmpeg-hwaccel) 

docker cp ${id}:/usr/bin/ffmpeg .

# Destroy the container
docker rm ${id}