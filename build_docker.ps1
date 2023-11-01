# Measure the time taken for the entire operation
$overallTime = Measure-Command {
    $gitVersion = "dotnet-gitversion"
    &$gitVersion /updateAssemblyInfo

    $json = &$gitVersion /output json
    $obj = $json | ConvertFrom-Json 
    $semVer = $obj.SemVer
    $buildMetaDataPadded = $obj.BuildMetaDataPadded
    $version = $obj.EscapedBranchName + "-" + $semVer

    Write-Output "Setting version to $version"

    $env:DOCKER_BUILDKIT = 1
    $env:COMPOSE_DOCKER_CLI_BUILD = 1

    # Multiple tags
    $tags = "docker.io/senexcrenshaw/streammaster:latest",
            "docker.io/senexcrenshaw/streammaster:$semVer",
            "docker.io/senexcrenshaw/streammaster:$semVer-$buildMetaDataPadded"

    Write-Output "Building and pushing with the following tags:"
    $tags | ForEach-Object { Write-Output $_ }

    # Build and push Docker image with multiple tags
    docker buildx build --platform linux/amd64,linux/arm64 -f ./Dockerfile . --push $(foreach ($tag in $tags) { "--tag=$tag" })
}

Write-Output "Overall time taken: $($overallTime.TotalSeconds) seconds"
