#!/bin/bash

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
select backup_file in $(ls $RESTORE_DIR/backup_*.tar.gz); do
    test -n "$backup_file" && break
    echo ">>> Invalid Selection"
done

echo "Selected backup file: $backup_file"

# Create temporary restore directory
mkdir -p "$temp_RESTORE_DIR"
tar -xzf "$RESTORE_DIR/$backup_file" -C "$temp_RESTORE_DIR"

# Extract database and files backup names
db_backup_file=$(ls $temp_RESTORE_DIR/db_*.gz)
files_backup_file=$(ls $temp_RESTORE_DIR/files_*.tar.gz)

# Restore PostgreSQL database
restore_database() {
    dropdb -U $POSTGRES_USER -h localhost -p 5432 $POSTGRES_DB --force
    createdb -U $POSTGRES_USER -h localhost -p 5432 -e $POSTGRES_DB
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
rm -rf "$temp_RESTORE_DIR"
rm -f "$RESTORE_DIR/$backup_file" 
echo "Restore process finished."
