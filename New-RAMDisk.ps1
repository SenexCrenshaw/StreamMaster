# Function to check if the script is running as administrator
function Test-Admin {
    $currentUser = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
    $currentUser.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

# Check for elevated permissions
if (-not (Test-Admin)) {
    Write-Warning "This script needs to be run as an administrator. Please restart the script with elevated privileges."
    Start-Process powershell -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs
    exit
}

# Function to create RAM disk and symbolic links
function New-RAMDisk {
    param (
        [int]$diskSizeGB,
        [string]$driveLetter
    )

    # Convert size to bytes
    $diskSizeBytes = $diskSizeGB * 1GB

    # Create RAM disk
    Invoke-Expression "imdisk -a -s $diskSizeBytes -m $driveLetter -p '/fs:ntfs /q /y'"

    # Confirm the RAM disk is created
    Get-PSDrive -PSProvider FileSystem

    # Link the RAM disk to C:\config\HLS
    if (Test-Path -Path "C:\config\HLS") {
        Remove-Item -Path "C:\config\HLS" -Recurse -Force
    }
    cmd /c mklink /D C:\config\HLS $driveLetter\HLS
}

# Function to remove RAM disk and symbolic links
function Remove-RAMDisk {
    param (
        [string]$driveLetter
    )

    # Remove symbolic links
    if (Test-Path -Path "C:\config\HLS") {
        Remove-Item -Path "C:\config\HLS" -Recurse -Force
    }

    # Remove RAM disk
    Invoke-Expression "imdisk -d -m $driveLetter"
}

# Main script logic
param (
    [switch]$Remove,
    [Int]$diskSizeGB,
    [string]$driveLetter = "R:"
)

if ($Remove) {
    Remove-RAMDisk -driveLetter $driveLetter
}
else {
    New-RAMDisk -diskSizeGB $diskSizeGB -driveLetter $driveLetter
}
