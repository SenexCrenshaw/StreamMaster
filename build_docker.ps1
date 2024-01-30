param (
    [switch]$DebugLog,
    [switch]$BuildProd,
    [switch]$PrintCommands = $false,
    [switch]$SkipRelease = $false
)

$env:DOCKER_BUILDKIT = 1
$env:COMPOSE_DOCKER_CLI_BUILD = 1

$imageName = "docker.io/senexcrenshaw/streammaster"

if ( !$SkipRelease) {
    npx semantic-release
}

# Dot-source the script to import the Get-AssemblyInfo function
. ".\Get-AssemblyInfo.ps1"

# Now you can call Get-AssemblyInfo from your main script
$result = Get-AssemblyInfo -assemblyInfoPath "./StreamMaster.API/AssemblyInfo.cs"

$result  |  Write-Output

$semVer = $result.Version
$buildMetaDataPadded = $result.Version
$branchName = $result.Branch

# if ($TagAndPush) {
#     # Stage all changes
#     git add -A

#     # Commit changes
#     git commit 

#     $json = &$gitVersion /output json | Out-String
#     $obj = $json | ConvertFrom-Json 
#     $semVer = $obj.SemVer
#     $buildMetaDataPadded = $obj.BuildMetaDataPadded
#     $branchName = $obj.BranchName

#     # # Tag the commit
#     # $tagName = "v$semVer-$buildMetaDataPadded"
#     # git tag -a $tagName -m "Release $tagName"

#     # Push commits to the remote repository
#     git push origin $branchName

#     # Push tag to the remote repository
#     # git push origin $tagName
# }
#$obj |  Write-Output

# Multiple tags
$tags = if ($BuildProd) {
    "${imageName}:latest",
    "${imageName}:$branchName-$semVer"
    # "${imageName}:$semVer-$buildMetaDataPadded"
}
else {
    "${imageName}:$branchName-$semVer"
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