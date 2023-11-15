param (
    [switch]$PrintOnly,
    [switch]$BuildProd,
    [switch]$DebugLog
)

$gitVersion = "dotnet-gitversion"
&$gitVersion /updateAssemblyInfo | Out-Null

$json = &$gitVersion /output json | Out-String
$obj = $json | ConvertFrom-Json 
$semVer = $obj.SemVer
$buildMetaDataPadded = $obj.AssemblySemVer
$branchName = $obj.BranchName

if ($PrintOnly -or $DebugLog) {
    Write-Output  $obj
}

$env:DOCKER_BUILDKIT = 1
$env:COMPOSE_DOCKER_CLI_BUILD = 1

# Multiple tags
$tags = if ($BuildProd) {
    "docker.io/senexcrenshaw/streammaster:latest",
    "docker.io/senexcrenshaw/streammaster:$semVer",
    "docker.io/senexcrenshaw/streammaster:$buildMetaDataPadded"
}
else {
    "docker.io/senexcrenshaw/streammaster:$branchName-$semVer-$buildMetaDataPadded"  
}

Write-Output "Tags to be used:"
$tags | ForEach-Object { Write-Output $_ }

if ($PrintOnly -or $DebugLog) {
    $buildCommand = "docker buildx build --platform ""linux/amd64,linux/arm64"" -f ./Dockerfile . --push " + ($tags | ForEach-Object { "--tag=$_" })
    Write-Output "Build Command: $buildCommand"
}

if ($PrintOnly) {
    Write-Output "PrintOnly flag is set. Exiting without building."
    exit
}

# Show the build command if either PrintOnly or DebugLog is set

# Capture the start time
$startTime = Get-Date

# Initialize line counter
$lineCounter = 0

# Prefix for the dots
Write-Host -NoNewline "Building Image "

# Skip build process if PrintOnly flag is set
if ($PrintOnly) {
    Write-Output "PrintOnly flag is set. Exiting without building."
    exit
}

# Run the build and push operation, displaying a dot for every 10 lines of output
try {
    docker buildx build --platform "linux/amd64,linux/arm64" -f ./Dockerfile . --push $(foreach ($tag in $tags) { "--tag=$tag" }) 2>&1 | ForEach-Object {
        if ($DebugLog) {
            $_ # Output the line for logging purposes if DebugLog flag is set
        }
        $lineCounter++
        if ($lineCounter % 10 -eq 0) {
            Write-Host -NoNewline "."
        }
    }
}
catch {
    Write-Error "Docker build failed with error: $_"
    exit 1
}

# Capture the end time
$endTime = Get-Date

# Calculate the total time taken
$overallTime = $endTime - $startTime

Write-Output "`nOverall time taken: $($overallTime.TotalSeconds) seconds"
