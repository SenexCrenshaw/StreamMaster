param (
    [switch]$DebugLog,
    [switch]$BuildAll = $false,
    [switch]$BuildBase = $false,
    [switch]$BuildBuild = $false,
    [switch]$BuildSM = $false,
    [switch]$Prod = $false,
    [switch]$PrintCommands = $false,
    [switch]$SkipRelease = $false,
    [switch]$SkipMainBuild = $false
)

$global:tags

function Write-StringToFile {
    param (
        [string]$Path,
        [string]$Content
    )
    try {
        Set-Content -Path $Path -Value $Content -NoNewline -Encoding UTF8
        Write-Host "Content written to file: $Path"
    }
    catch {
        Write-Host "An error occurred: $_"
    }
}

function Read-StringFromFile {
    param (
        [string]$Path
    )
    try {
        $Content = Get-Content -Path $Path -Raw
        Write-Host "Content read from file: $Path"
        return $Content
    }
    catch {
        Write-Host "An error occurred: $_"
        return $null
    }
}

function Copy-File {
    param (
        [Parameter(Mandatory=$true)]
        [string]$sourcePath,

        [Parameter(Mandatory=$true)]
        [string]$destinationPath
    )

    try {
        # Check if the source file exists
        if (-Not (Test-Path -Path $sourcePath -PathType Leaf)) {
            Write-Host "Source file does not exist: '$sourcePath'"
            return
        }

        # Copy the file to the destination
        Copy-Item -Path $sourcePath -Destination $destinationPath -ErrorAction Stop
        Write-Host "File copied successfully from '$sourcePath' to '$destinationPath'."
    } catch {
        Write-Host "An error occurred while copying the file: $_"
    }
}

function Main {
    Set-EnvironmentVariables

   

    if (  $SkipMainBuild) {
        $Prod = $false
    }

    if (-not $SkipRelease -and -not $PrintCommands) {

        if ( $Prod ) { #} -and -not $SkipMainBuild) {
            Copy-File -sourcePath "release.config.release.cjs" -destinationPath "release.config.cjs"
        }
        else {
            Copy-File -sourcePath "release.config.norelease.cjs" -destinationPath "release.config.cjs"
        }

        npx semantic-release
    }

    # DownloadFiles

    $imageName = "docker.io/senexcrenshaw/streammaster"
    $buildName = "streammaster-builds"

    $result = Get-AssemblyInfo -assemblyInfoPath "./StreamMaster.API/AssemblyInfo.cs"
    $processedAssemblyInfo = ProcessAssemblyInfo $result

    if ($BuildBase -or $BuildAll) {
        $dockerFile = "Dockerfile.base"
        $global:tags = @("$("${buildName}:"+$processedAssemblyInfo.BranchNameRevision)-base")
        BuildImage -result $processedAssemblyInfo -imageName $buildName -dockerFile $dockerFile -pull $true
        Write-StringToFile -Path "basever" -Content $processedAssemblyInfo.BranchNameRevision 
    }

    if ($BuildBuild -or $BuildAll) {
        $dockerFile = "Dockerfile.build"
        $global:tags = @("$("${buildName}:"+$processedAssemblyInfo.BranchNameRevision)-build")
        BuildImage -result $processedAssemblyInfo -imageName $buildName -dockerFile $dockerFile -pull $true
        Write-StringToFile -Path "buildver" -Content $processedAssemblyInfo.BranchNameRevision 
    }
    
    if ($BuildSM -or $BuildAll) {
        $dockerFile = "Dockerfile.sm"
        $global:tags = @("$("${buildName}:"+$processedAssemblyInfo.BranchNameRevision)-sm")
        
        $buildver = Read-StringFromFile -Path "buildver";      
        $contentArray = @('FROM --platform=$BUILDPLATFORM ' + "${buildName}:$($buildver)-build" + ' AS build');
        Add-ContentAtTop -filePath  $dockerFile -contentArray $contentArray

        $smver = $processedAssemblyInfo.BranchNameRevision ;

        BuildImage -result $processedAssemblyInfo -imageName $buildName -dockerFile $dockerFile
        Write-StringToFile -Path "smver" -Content $smver
    }

    if ( -not $SkipMainBuild -or $BuildAll) {
        $dockerFile = "Dockerfile"

        $contentArray = @();

        $basever = Read-StringFromFile -Path "basever";
        $smver = Read-StringFromFile -Path "smver";

        $contentArray += 'FROM ' + "${buildName}:$($smver)-sm" + ' AS sm'      
        $contentArray += 'FROM ' + "${buildName}:$($basever)-base" + ' AS base'  
        
        Add-ContentAtTop -filePath  $dockerFile -contentArray $contentArray

        $global:tags = DetermineTags -result $processedAssemblyInfo -imageName $imageName
        BuildImage -result $processedAssemblyInfo -imageName $imageName -dockerFile $dockerFile -push $true
    }
}
Function Add-ContentAtTop {
    param(
        [string]$filePath,
        [string[]]$contentArray
    )

    $templatePath = "$filePath.template"

    # Check if the file exists
    if (Test-Path $templatePath) {
        try {
            # Read all lines of the existing content of the file
            $existingContent = [System.IO.File]::ReadAllLines($templatePath)

            # Initialize a new array list
            $newContent = New-Object System.Collections.ArrayList

            # Add each line from contentArray to the new content
            foreach ($line in $contentArray) {
                Write-Output $line
                [void]$newContent.Add($line)
            }

            # Add existing content
            $existingContent | ForEach-Object { [void]$newContent.Add($_) }

            # Write the new content to the file
            [System.IO.File]::WriteAllLines($filePath, $newContent)
            Write-Output "Content added successfully at the top of the file."
        }
        catch {
            Write-Error "An error occurred: $_"
        }
    }
    else {
        Write-Error "The file '$filePath' does not exist."
    }
}

function Set-EnvironmentVariables {
    $env:DOCKER_BUILDKIT = 1
    $env:COMPOSE_DOCKER_CLI_BUILD = 1

     # Read GitHub token and set it as an environment variable
    $ghtoken = Get-Content ghtoken -Raw
    Write-Output "GitHub Token: $ghtoken"
    $env:GH_TOKEN = $ghtoken
}

function DownloadFiles {
    curl -s 'https://raw.githubusercontent.com/docker-library/postgres/master/docker-entrypoint.sh' -o .\docker-entrypoint.sh  
    curl -s 'https://raw.githubusercontent.com/docker-library/postgres/master/docker-ensure-initdb.sh' -o .\docker-ensure-initdb.sh
}

function ProcessAssemblyInfo($result) {
    # Output the raw assembly information (as received in $result)    
    Write-Host "Raw Assembly Information:"
    $result | Write-Host

    # Ensuring the output is properly flushed to the console
    [System.Console]::Out.Flush()

    # Extracting and processing specific parts of the assembly information
    $semVer = $result.Version
    Write-Host "Semantic Version: $semVer"

    # Check if Branch is empty or 'N/A'
    if ([string]::IsNullOrEmpty($result.Branch) -or $result.Branch -eq 'N/A') {
        $branchName = $semVer
        $branchNameVersion = $semVer
    }
    else {
        $branchName = $result.Branch
        $branchNameVersion = "$($result.Branch)-$semVer"
    }

    Write-Host "Branch Name: $branchName"
    Write-Host "Branch Name Version: $branchNameVersion"
    $BranchNameRevision = $branchName;
    # Process build or revision if available and not 'N/A'
    if (![string]::IsNullOrEmpty($result.BuildOrRevision) -and $result.BuildOrRevision -ne 'N/A') {
        $BranchNameRevision = "$branchNameVersion.$($result.BuildOrRevision)"
        Write-Host "Branch Name with Build or Revision: $BranchNameRevision"
    }

    # Return the processed information as a custom object for further use
    $processedInfo = New-Object -TypeName PSObject -Property @{
        SemVer             = $semVer
        BranchName         = $branchName
        BranchNameVersion  = $branchNameVersion
        BranchNameRevision = $BranchNameRevision
        # Include other properties as needed
    }

    return $processedInfo
}

function BuildImage {
    param (
        [Parameter(Mandatory = $true)]
        $result,

        [Parameter(Mandatory = $true)]
        $dockerFile,

        [Parameter(Mandatory = $true)]
        [string]$imageName,

        [Parameter()]
        [bool]$push = $false,

        [Parameter()]
        [bool]$pull = $false

    )
  
    # Show the tags to be used
    Write-Host "Tags to be used:"
    $global:tags | ForEach-Object { Write-Host $_ }

    # Construct the Docker build command using the tags and the specified Dockerfile
    $buildCommand = "docker buildx build "
     if ( $pull) {
        $buildCommand += " --pull"
    }
    $buildCommand += " --platform ""linux/amd64,linux/arm64"" -f ./$dockerFile ."

    if ( $push) {
        $buildCommand += " --push"
    }
    else {      
        $buildCommand += " --load"
    }
    foreach ($tag in $global:tags) {
        $buildCommand += " --tag=$tag"
    }

    if ($PrintCommands) {
        Write-Host "Build Command: $buildCommand"
        Write-Host "PrintOnly flag is set. Exiting without building."
        return
    }

    Invoke-Build $buildCommand
}

function DetermineTags {
    param (
        [Parameter(Mandatory = $true)]
        $result,

        [Parameter(Mandatory = $true)]
        [string]$imageName
    )
   
    $BranchName = $result.BranchName
    $BranchNameRevision = $result.BranchNameRevision

    $global:tags = @()
    if ($Prod) {
        $global:tags += "${imageName}:latest"
    }
    else {
        $global:tags += "${imageName}:${BranchName}"
    }
    $global:tags += "${imageName}:${BranchNameRevision}"
    return $global:tags
}

function ConstructBuildCommand() {
    $buildCommand = "docker buildx build --platform ""linux/amd64,linux/arm64"" -f ./Dockerfile.orig . --push"
    foreach ($tag in $global:tags) {
        $buildCommand += " --tag=$global:tags"
    }
    return $buildCommand
}

function Invoke-Build($buildCommand) {
    $startTime = Get-Date
    Write-Host -NoNewline "Building Image "

    Invoke-Expression $buildCommand

    $endTime = Get-Date
    $overallTime = $endTime - $startTime
    Write-Host "`nOverall time taken: $($overallTime.TotalSeconds) seconds"
}

. ".\Get-AssemblyInfo.ps1"

# Entry point of the script
Main

Write-Host "Tags to be used:"
$global:tags | ForEach-Object { Write-Host $_ }