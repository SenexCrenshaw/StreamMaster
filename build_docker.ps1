param (
    [switch]$DebugLog,
    [switch]$BuildProd = $false,
    [switch]$PrintCommands = $false,
    [switch]$SkipRelease = $false
)

$env:DOCKER_BUILDKIT = 1
$env:COMPOSE_DOCKER_CLI_BUILD = 1

$ghtoken = Get-Content ghtoken -Raw
$env:GH_TOKEN = $ghtoken

$imageName = "docker.io/senexcrenshaw/streammaster"

if (-not $SkipRelease ) {
    #-and $BuildProd) {
    # Write-Host $SkipRelease;
    # Write-Host $BuildProd;
    npx semantic-release
}


. ".\Get-AssemblyInfo.ps1"

$result = Get-AssemblyInfo -assemblyInfoPath "./StreamMaster.API/AssemblyInfo.cs"

$result  |  Write-Output
[System.Console]::Out.Flush()

$semVer = $result.Version
# $buildMetaDataPadded = $result.Version
# Check if Branch is empty or 'N/A'
if ([string]::IsNullOrEmpty($result.Branch) -or $result.Branch -eq 'N/A') {
    $branchName = $semVer
}
else {
    # If branch is present, append it to the semVer
    $branchName = "$($result.Branch)-$semVer"
}

if (![string]::IsNullOrEmpty($result.BuildOrRevision) ) {
    $branchName = "$branchName.$($result.BuildOrRevision)"
}

# Multiple tags
$tags = if ($BuildProd) {
    "${imageName}:latest",
    "${imageName}:$branchName"
    # "${imageName}:$semVer-$buildMetaDataPadded"
}
else {
    "${imageName}:$branchName"
}

Write-Output "Tags to be used:"
$tags | ForEach-Object { Write-Output $_ }
$buildCommand = "docker buildx build --platform ""linux/amd64,linux/arm64"" -f ./Dockerfile . --push " + ($tags | ForEach-Object { "--tag=$_" })

if ($PrintCommands) {    
    Write-Output "Build Command: $buildCommand"
}

# Capture the start time
$startTime = Get-Date

# Prefix for the dots
Write-Host -NoNewline "Building Image "

# Skip build process if PrintOnly flag is set
if ($PrintCommands) {
    Write-Output "PrintOnly flag is set. Exiting without building."
    exit
}

Invoke-Expression $buildCommand

# Capture the end time
$endTime = Get-Date

# Calculate the total time taken
$overallTime = $endTime - $startTime

Write-Output "`nOverall time taken: $($overallTime.TotalSeconds) seconds"