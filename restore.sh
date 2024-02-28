#!/bin/bash

# Configuration
restore_path="/config/DB/Restore"
temp_restore_path="/tmp/restore_temp"
timestamp=$(date +"%Y-%m-%d_%H-%M-%S")

# Ensure the restore path exists
if [ ! -d "$restore_path" ]; then
    echo "Restore path does not exist: $restore_path"
    exit 0
fi

# List available backups and prompt user for selection
echo "Available backups:"
select backup_file in $(ls $restore_path/backup_*.tar.gz); do
    test -n "$backup_file" && break
    echo ">>> Invalid Selection"
done

echo "Selected backup file: $backup_file"

# Create temporary restore directory
mkdir -p "$temp_restore_path"
tar -xzf "$restore_path/$backup_file" -C "$temp_restore_path"

# Extract database and files backup names
db_backup_file=$(ls $temp_restore_path/db_*.gz)
files_backup_file=$(ls $temp_restore_path/files_*.tar.gz)

# Restore PostgreSQL database
restore_database() {
    gzip -d < "$db_backup_file" | psql -U $POSTGRES_USER $POSTGRES_DB
    echo "Database restore completed."
}

# Restore files and directories
restore_files_and_dirs() {
    tar -xzf "$files_backup_file" -C /
    echo "Directories and files restore completed."
}

# Perform the restore operations
echo "Starting restore process..."
restore_database
restore_files_and_dirs

# Cleanup temporary files
rm -rf "$temp_restore_path"
echo "Restore process finished."
