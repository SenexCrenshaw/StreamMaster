param (
    [switch]$PrintOnly,
    [switch]$BuildTest
)

$gitVersion = "dotnet-gitversion"
&$gitVersion /updateAssemblyInfo | Out-Null

$json = &$gitVersion /output json | Out-String
$obj = $json | ConvertFrom-Json 
$semVer = $obj.SemVer
$buildMetaDataPadded = $obj.BuildMetaDataPadded
$branchName = $obj.BranchName
$version = $obj.EscapedBranchName + "-" + $semVer

Write-Output "Setting version to $version"

$env:DOCKER_BUILDKIT = 1
$env:COMPOSE_DOCKER_CLI_BUILD = 1

# Multiple tags
$tags = if ($BuildTest) {
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

# Capture the start time
$startTime = Get-Date

# Initialize line counter
$lineCounter = 0

# Prefix for the dots
Write-Host -NoNewline "Building Image "

# Run the build and push operation, displaying a dot for every 10 lines of output
docker buildx build --platform linux/amd64,linux/arm64 -f ./Dockerfile . --push $(foreach ($tag in $tags) { "--tag=$tag" }) 2>&1 | ForEach-Object { 
    $lineCounter++
    if ($lineCounter % 10 -eq 0) {
        Write-Host -NoNewline "."
    }
}

# Capture the end time
$endTime = Get-Date

# Calculate the total time taken
$overallTime = $endTime - $startTime

Write-Output "`nOverall time taken: $($overallTime.TotalSeconds) seconds"
