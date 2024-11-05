#!/bin/bash

# Source environment variables
. /env.sh

# Configuration
temp_RESTORE_DIR=$(mktemp -d -t restore.XXXXXX)
timestamp=$(date +"%Y-%m-%d_%H-%M-%S")

# Ensure the restore path exists
if [ ! -d "$RESTORE_DIR" ]; then
    echo "Restore path does not exist: $RESTORE_DIR"
    exit 0
fi

# List available backups and prompt user for selection
echo "Available backups:"
files=("$RESTORE_DIR"/backup_*.tar.gz)
if [ ${#files[@]} -eq 0 ] || [ ! -e "${files[0]}" ]; then
    echo "No backup files found in $RESTORE_DIR."
    return 1
fi

file_found=0 # Default to no files found (0 means files ready)
for backup_file in "${files[@]}"; do
    if [ -s "$backup_file" ]; then
        echo "File is ready for restore: $(basename "$backup_file")"
        file_found=1 # Set to 0 if a file is found and is not empty
        break
    else
        echo "Found an empty backup file: $(basename "$backup_file")"
    fi
done

if [ $file_found -eq 0 ]; then
    echo "No backup files are ready for restore."
    exit 0
fi

# Create temporary restore directory
mkdir -p "$temp_RESTORE_DIR"
tar -xzf "$backup_file" -C "$temp_RESTORE_DIR"

# Extract database and files backup names
db_backup_file=$(ls $temp_RESTORE_DIR/db_*.gz)
files_backup_file=$(ls $temp_RESTORE_DIR/files_*.tar.gz)

# Restore PostgreSQL database
restore_database() {
    dropdb -U $POSTGRES_USER -h $POSTGRES_HOST -p 5432 $POSTGRES_DB --force
    createdb -U $POSTGRES_USER -h $POSTGRES_HOST -p 5432 -e $POSTGRES_DB
    gzip -d <"$db_backup_file" | psql -h $POSTGRES_HOST -U $POSTGRES_USER $POSTGRES_DB
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
rm -rf "$temp_RESTORE_DIR"
rm -f "$backup_file"
echo "Restore process finished."
