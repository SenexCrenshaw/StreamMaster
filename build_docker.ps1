param (
    [switch]$DebugLog,
    [switch]$BuildProd,
    [switch]$PrintCommands
)

$env:DOCKER_BUILDKIT = 1
$env:COMPOSE_DOCKER_CLI_BUILD = 1

# Define the base image name
$imageName = "docker.io/senexcrenshaw/streammaster"

$gitVersion = "dotnet-gitversion"
&$gitVersion /updateAssemblyInfo | Out-Null

$json = &$gitVersion /output json | Out-String
$obj = $json | ConvertFrom-Json 
$semVer = $obj.SemVer
$buildMetaDataPadded = $obj.BuildMetaDataPadded
$branchName = $obj.BranchName

$obj |  Write-Output

# Multiple tags
$tags = if ($BuildProd) {
    "${imageName}:latest",
    "${imageName}:$semVer",
    "${imageName}:$semVer-$buildMetaDataPadded"
}
else {
    "${imageName}:$branchName-$semVer-$buildMetaDataPadded"
}

Write-Output "Tags to be used:"
$tags | ForEach-Object { Write-Output $_ }
$buildCommand = "docker buildx build --platform ""linux/amd64,linux/arm64"" -f ./Dockerfile . --push " + ($tags | ForEach-Object { "--tag=$_" })

if ($PrintCommands) {    
    Write-Output "Build Command: $buildCommand"
}
# Show the build command if either PrintOnly or DebugLog is set

# Capture the start time
$startTime = Get-Date

# Initialize line counter
# $lineCounter = 0

# Prefix for the dots
Write-Host -NoNewline "Building Image "

# Skip build process if PrintOnly flag is set
if ($PrintCommands) {
    Write-Output "PrintOnly flag is set. Exiting without building."
    exit
}

Invoke-Expression $buildCommand

# # Run the build and push operation, displaying a dot for every 10 lines of output
# try {
#     Invoke-Expression $buildCommand 2>&1 | ForEach-Object {
#         if ($DebugLog) {
#             Write-Host $_ # Output the line for logging purposes if DebugLog flag is set
#         }
#         else {
#             $lineCounter++
#             if ($lineCounter % 10 -eq 0) {
#                 Write-Host -NoNewline "."
#             }
#         }
#     }
# }
# catch {
#     Write-Error "Docker build failed with error: $_"
#     exit 1
# }

# Capture the end time
$endTime = Get-Date

# Calculate the total time taken
$overallTime = $endTime - $startTime

Write-Output "`nOverall time taken: $($overallTime.TotalSeconds) seconds"