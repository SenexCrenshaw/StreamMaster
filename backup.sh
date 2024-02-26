#!/bin/bash

# Source environment variables
. /env.sh

# Configuration
backup_dirs="/config/PlayLists /config/Logs /config/Cache/SDJson /config/Settings" # Space-separated list of directories
backup_files="/config/settings.json /config/logsettings.json" # Space-separated list of files
backup_path="/config/Backups"
versions_to_keep=5
timestamp=$(date +"%Y-%m-%d_%H-%M-%S")
db_backup_file="db_$timestamp.gz"
files_backup_file="files_$timestamp.tar.gz"
backup_file="$backup_path/backup_$timestamp.tar.gz"

# Determine versions to keep from command line argument, environment variable, or default
if [ ! -z "$1" ]; then
    versions_to_keep=$1
elif [ ! -z "$BACKUP_VERSIONS_TO_KEEP" ]; then
    versions_to_keep=$BACKUP_VERSIONS_TO_KEEP
else
    versions_to_keep=5 
fi

# Check if backup directory exists, if not, create it
if [ ! -d "$backup_path" ]; then
    mkdir -p "$backup_path"
fi

# Backup PostgreSQL database
backup_database() {
    pg_dump -U $POSTGRES_USER $POSTGRES_DB | gzip > "$backup_path/$db_backup_file"
    echo "Database backup completed: $db_backup_file"
}

# Backup files and directories
backup_files_and_dirs() {
    local items_to_backup=()
    for item in $backup_dirs $backup_files; do
        if [ -e "$item" ]; then
            items_to_backup+=("$item")
        else
            echo "Warning: $item does not exist and will not be included in the backup."
        fi
    done
    
    if [ ${#items_to_backup[@]} -gt 0 ]; then
        tar -czf "$backup_path/$files_backup_file" "${items_to_backup[@]}"  2>/dev/null
        echo "Directories and files backup completed: $files_backup_file"
    else
        echo "No files or directories exist for backup."
    fi
}

# Create one backup file
create_backup() {
    tar -czf "$backup_file" -C "$backup_path" $db_backup_file $files_backup_file  2>/dev/null
    echo "Backup file created: $backup_file"
}

# Function to limit the number of backups
limit_backups() {
    cd $backup_path
    # Remove individual db and files backups to clean up
    rm -f $db_backup_file $files_backup_file
    # Keep only the specified number of backup versions
    (ls -t backup_*.tar.gz | head -n $versions_to_keep; ls backup_*.tar.gz) | sort | uniq -u | xargs --no-run-if-empty rm
    echo "Old backups cleanup completed."
}

# Main script execution
echo "Starting backup process..."
backup_database
backup_files_and_dirs
create_backup
limit_backups
echo "Backup process finished."
