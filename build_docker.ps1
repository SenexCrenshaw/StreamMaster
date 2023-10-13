.\UpdateVersions.ps1
$version = Get-Content "Version"
$dockerCommand = ".\build_docker.bat"
&$dockerCommand $version

$env:DOCKER_BUILDKIT = 1
$env:COMPOSE_DOCKER_CLI_BUILD = 1
docker build --pull --rm -f "Dockerfile" -t streammaster:latest "."

docker buildx build --platform linux/amd64,linux/arm64 -t docker.io/senexcrenshaw/streammaster:test -f ./Dockerfile . --push