# Source environment variables
# PowerShell equivalent might require directly setting variables or using a .ps1 file for environment variables.

# Configuration
$backupDirs = @("/config/PlayLists", "/config/Logs", "/config/Cache/SDJson")
$backupFiles = @("/config/settings.json", "/config/logsettings.json")
$backupPath = "/config/backups"
$versionsToKeep = 5
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$dbBackupFile = "db_$timestamp.gz"
$filesBackupFile = "files_$timestamp.tar.gz"
$backupFile = "$backupPath/backup_$timestamp.tar.gz"

$env:BACKUP_VERSIONS_TO_KEEP = 18
$env:POSTGRES_USER = "sm"
$env:POSTGRES_DB = "StreamMaster"
$env:POSTGRES_PASSWORD = "sm123"

# Determine versions to keep from command line argument, environment variable, or default
if ($args.Count -gt 0) {
    $versionsToKeep = $args[0]
}
elseif ([Environment]::GetEnvironmentVariable("BACKUP_VERSIONS_TO_KEEP")) {
    $versionsToKeep = [Environment]::GetEnvironmentVariable("BACKUP_VERSIONS_TO_KEEP")
}

# Check if backup directory exists, if not, create it
if (-not (Test-Path $backupPath)) {
    New-Item -Path $backupPath -ItemType Directory
}

# Backup PostgreSQL database
function Backup-Database {
    & pg_dump -U $env:POSTGRES_USER $env:POSTGRES_DB | Compress-Archive -DestinationPath "$backupPath/$dbBackupFile"
    Write-Host "Database backup completed: $dbBackupFile"
}

# Backup files and directories
function Backup-FilesAndDirs {
    $itemsToBackup = @()
    foreach ($item in $backupDirs + $backupFiles) {
        if (Test-Path $item) {
            $itemsToBackup += $item
        }
        else {
            Write-Host "Warning: $item does not exist and will not be included in the backup."
        }
    }
    
    if ($itemsToBackup.Count -gt 0) {
        Compress-Archive -Path $itemsToBackup -DestinationPath "$backupPath/$filesBackupFile"
        Write-Host "Directories and files backup completed: $filesBackupFile"
    }
    else {
        Write-Host "No files or directories exist for backup."
    }
}

# Create one backup file
function Create-Backup {
    Compress-Archive -Path "$backupPath/$dbBackupFile", "$backupPath/$filesBackupFile" -DestinationPath $backupFile
    Write-Host "Backup file created: $backupFile"
}

# Function to limit the number of backups
function Limit-Backups {
    Set-Location $backupPath
    # Remove individual db and files backups to clean up
    Remove-Item $dbBackupFile, $filesBackupFile -ErrorAction SilentlyContinue
    # Keep only the specified number of backup versions
    Get-ChildItem -Filter "backup_*.tar.gz" | Sort-Object CreationTime -Descending | Select-Object -Skip $versionsToKeep | Remove-Item
    Write-Host "Old backups cleanup completed."
}

# Main script execution
Write-Host "Starting backup process..."
Backup-Database
Backup-FilesAndDirs
Create-Backup
Limit-Backups
Write-Host "Backup process finished."
