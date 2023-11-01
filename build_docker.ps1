param (
    [switch]$PrintOnly,
    [switch]$Test
)

$gitVersion = "dotnet-gitversion"
&$gitVersion /updateAssemblyInfo

$json = &$gitVersion /output json
$obj = $json | ConvertFrom-Json 
$semVer = $obj.SemVer
$buildMetaDataPadded = $obj.BuildMetaDataPadded
$branchName = $obj.BranchName
$version = $obj.EscapedBranchName + "-" + $semVer

Write-Output "Setting version to $version"

$env:DOCKER_BUILDKIT = 1
$env:COMPOSE_DOCKER_CLI_BUILD = 1

# Multiple tags
$tags = if ($Test) {
    "docker.io/senexcrenshaw/streammaster:$branchName-$semVer-$buildMetaDataPadded"
} else {
    "docker.io/senexcrenshaw/streammaster:latest",
    "docker.io/senexcrenshaw/streammaster:$semVer",
    "docker.io/senexcrenshaw/streammaster:$semVer-$buildMetaDataPadded"
}

Write-Output "Tags to be used:"
$tags | ForEach-Object { Write-Output $_ }

if ($PrintOnly) {
    Write-Output "PrintOnly flag is set. Exiting without building."
    exit
}

# Measure the time taken for the build and push operation
$overallTime = Measure-Command {
    docker buildx build --platform linux/amd64,linux/arm64 -f ./Dockerfile . --push $(foreach ($tag in $tags) { "--tag=$tag" }) 2>&1 | Tee-Object -Variable dockerOutput
}

# Output the captured Docker output
$dockerOutput

Write-Output "Overall time taken: $($overallTime.TotalSeconds) seconds"
