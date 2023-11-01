$gitVersion = "dotnet-gitversion"
&$gitVersion /updateAssemblyInfo

$json = &$gitVersion /output json
$obj = $json | ConvertFrom-Json 
$version = $obj.EscapedBranchName + "-" + $obj.BuildMetaDataPadded 

Write-Output "Setting version to $version"

$env:DOCKER_BUILDKIT = 1
$env:COMPOSE_DOCKER_CLI_BUILD = 1

docker buildx build --platform linux/amd64,linux/arm64 -t docker.io/senexcrenshaw/streammaster:$version -f ./Dockerfile . --push