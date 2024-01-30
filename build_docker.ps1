param (
    [switch]$DebugLog,
    [switch]$BuildProd,
    [switch]$PrintCommands = $false
    # [switch]$TagAndPush = $false
)

$env:DOCKER_BUILDKIT = 1
$env:COMPOSE_DOCKER_CLI_BUILD = 1

# Define the path to your AssemblyInfo.cs file
$assemblyInfoPath = "./StreamMaster.API/AssemblyInfo.cs"

# Read the content of the AssemblyInfo.cs file
$content = Get-Content $assemblyInfoPath -Raw

# Define the regex pattern for AssemblyInformationalVersion with branch and SHA capture
$assemblyInformationalVersionPattern = '\[assembly: AssemblyInformationalVersion\("([0-9.]+)-([^.]+)\.[0-9]+\.Sha\.([a-fA-F0-9]+)"\)\]'

# Find matches in the content
$assemblyInformationalVersionMatch = [regex]::Match($content, $assemblyInformationalVersionPattern)

# Extract and display the version numbers, branch, and SHA
if ($assemblyInformationalVersionMatch.Success) {
    $version = $assemblyInformationalVersionMatch.Groups[1].Value
    $branch = $assemblyInformationalVersionMatch.Groups[2].Value
    $sha = $assemblyInformationalVersionMatch.Groups[3].Value

    "Version: $version"
    "Branch: $branch"
    "Sha: $sha"
}
else {
    "Version information not found in the file."
}


$semVer = $assemblyVersion
$buildMetaDataPadded = $assemblyInformationalVersion
$branchName = $branch

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