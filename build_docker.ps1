param (
    [switch]$PrintOnly,
    [switch]$BuildProd,
    [switch]$DebugLog,
    [switch]$ManiFestOnly,
    [switch]$BuildBase
)

$gitVersion = "dotnet-gitversion"
&$gitVersion /updateAssemblyInfo | Out-Null

$json = &$gitVersion /output json | Out-String
$obj = $json | ConvertFrom-Json
$semVer = $obj.SemVer
$buildMetaDataPadded = $obj.AssemblySemVer
$branchName = $obj.BranchName

if ($PrintOnly -or $DebugLog) {
    Write-Output $obj
}

$env:DOCKER_BUILDKIT = 1
$env:COMPOSE_DOCKER_CLI_BUILD = 1

# Multiple tags
$tags = if ($BuildProd) {
    @(
        "docker.io/senexcrenshaw/streammaster:latest",
        "docker.io/senexcrenshaw/streammaster:$semVer",
        "docker.io/senexcrenshaw/streammaster:$buildMetaDataPadded"
    )
}
elseif ($BuildBase) {
    @(
        "docker.io/senexcrenshaw/streammaster_base:latest",
        "docker.io/senexcrenshaw/streammaster_base:$buildMetaDataPadded"
    )
}
else {
    @( "docker.io/senexcrenshaw/streammaster:$branchName-$semVer-$buildMetaDataPadded")
}

# Write-Output "Tags to be used:"
$tags | ForEach-Object { Write-Output $_ }

if ($PrintOnly) {
    Write-Output "PrintOnly flag is set. Exiting without building."
    exit
}

# Define a script block for the Docker build process
$dockerBuildAndPushScript = {
    param($platform, $dockerfile, $baseTag, $debugLog, $buildProd)
    $platformTag = $platform -replace '\/', '-'  # Replacing slash with dash
    if ( $buildProd ) {
        $imageTag = "$baseTag"
    }
    else {
        $imageTag = "$baseTag-$platformTag" 
    }    
    
    $command = "docker buildx build --build-arg FFMPEG_BASE_IMAGE_TAG=$platformTag --platform $platform -f $dockerfile . --tag $imageTag --push"
    Write-Output "Executing command: $command"
    & cmd.exe /c $command
}

# Start the build jobs
$jobs = @()

# Capture the start time
$startTime = Get-Date

# Start the build and push jobs for each architecture
if (!$ManiFestOnly) {
    if ( $BuildProd ) {
        $command = "docker buildx build --build-arg FFMPEG_BASE_IMAGE_TAG=$platformTag --platform $platform -f $dockerfile . --tag $imageTag --push"
        Write-Output "Starting job for platform: $platform and tag: $tag with command: $command"
        $jobs += Start-Job -ScriptBlock $dockerBuildAndPushScript -ArgumentList $platform, $dockerfile, $tag, $DebugLog, $BuildProd 
    }
    else {

        $platforms = @('linux/amd64', 'linux/arm64')
        foreach ($platform in $platforms) {
            $dockerfile = "Dockerfile." + $platform -replace '/', '.'  # Example: Dockerfile.linux.amd64
            foreach ($tag in $tags) {
                $platformTag = $platform -replace '\/', '-'  # Replacing slash with dash
          
                if ( $BuildProd ) {
                    $imageTag = "$baseTag"
                }
                else {
                    $imageTag = "$baseTag-$platformTag" 
                }    

                $command = "docker buildx build --build-arg FFMPEG_BASE_IMAGE_TAG=$platformTag --platform $platform -f $dockerfile . --tag $imageTag --push"
                Write-Output "Starting job for platform: $platform and tag: $tag with command: $command"
                $jobs += Start-Job -ScriptBlock $dockerBuildAndPushScript -ArgumentList $platform, $dockerfile, $tag, $DebugLog, $BuildProd 
            }
        }
    }

    # Implement a progress timer while jobs are running
    do {
        Start-Sleep -Seconds 1  # Update every second

        $elapsed = (Get-Date) - $startTime
        $totalSeconds = [Math]::Round($elapsed.TotalSeconds)
        Write-Host -NoNewline "`rElapsed time: $totalSeconds seconds    "
    } while ($jobs | Where-Object { $_.State -eq 'Running' })

    Write-Host ""  # Move to the next line after the loop completes

    # Wait for all jobs to complete
    $jobs | Wait-Job

    # Retrieve and display results from the jobs
    $jobs | ForEach-Object {
        Receive-Job -Job $_
        # Write-Output $buildOutput
        Remove-Job -Job $_  # Clean up the job
    }
}
# Create and push multi-architecture manifest for each tag
# if ($BuildBase) {
#     $platforms = @('linux/amd64', 'linux/arm64')
#     foreach ($tag in $tags) {
       
#         Write-output $tag

#         $manifestImages = $platforms | ForEach-Object { "$tag-" + ($_ -replace '/', '-') }

#         Write-output $manifestImages
#         $manifestCommand = "docker manifest create $tag " + ($manifestImages -join " $tag")
#         Write-Output "Executing command: $manifestCommand"  
#         & cmd.exe /c $manifestCommand

#         $pushManifestCommand = "docker manifest push $tag"
#         Write-Output "Executing command: $pushManifestCommand"  
#         & cmd.exe /c $pushManifestCommand
           
#         Write-Output "Manifest created and pushed for tag: $imageTag"
#     }    
# }

# Capture the end time
$endTime = Get-Date

# Calculate the total time taken
$overallTime = $endTime - $startTime

Write-Output "`nOverall time taken: $($overallTime.TotalSeconds) seconds"
