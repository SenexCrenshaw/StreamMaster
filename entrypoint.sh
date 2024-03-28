#!/bin/bash

user_name="nonRootUser"
group_name="nonRootGroup"

. /env.sh

moveFilesAndDeleteDir() {
    local source_dir=$1
    local destination_dir=$2

    # Check if the source directory exists
    if [ -d "$source_dir" ]; then
        # Ensure the destination directory exists, create if not
        if [ ! -d "$destination_dir" ]; then
            mkdir -p "$destination_dir"
        fi

        # Check if there are any files or directories in source_dir
        local files=("$source_dir"/*)
        if [ -e "${files[0]}" ]; then
            # Move files from source to destination
            mv "$source_dir"/* "$destination_dir"/ 2>/dev/null

            # Check if move operation was successful
            if [ $? -eq 0 ]; then
                echo "Files moved successfully from $source_dir to $destination_dir."
            else
                echo "Failed to move files from $source_dir to $destination_dir."
                return  # Exit the function if moving files failed
            fi
        else
            echo "No files to move from $source_dir."
        fi

        # Remove the source directory
        rmdir "$source_dir" 2>/dev/null
        if [ $? -eq 0 ]; then
            echo "Source directory $source_dir removed."
        else
            echo "Failed to remove source directory $source_dir. It might not be empty."
        fi
    fi
}

# Function to check for any file ready to be restored in /config/DB/Restore
check_files_ready_for_restore() {    
    local file_found=0

    # Check if the restore directory exists
    if [ ! -d "$RESTORE_DIR" ]; then
        echo "Restore path does not exist: $RESTORE_DIR"
        return 1
    fi

    # Initialize an array to hold the files that match the pattern
    local files=("$RESTORE_DIR"/backup_*.tar.gz)

    # Check if files array is empty (meaning no files found)
    if [ ${#files[@]} -eq 0 ] || [ ! -e "${files[0]}" ]; then
        echo "No backup files found in $RESTORE_DIR."
        return 1
    fi

    # Iterate over files matching the expected backup file pattern
    for file in "${files[@]}"; do
        # Check if file is not empty
        if [ -s "$file" ]; then
            echo "File is ready for restore: $(basename "$file")"
            file_found=1
            break # Break after finding the first ready file
        else
            echo "Found an empty backup file: $(basename "$file")"
        fi
    done

    return $file_found
}

rename_directory() {
    local src="$1"
    local dest="$2"

    # Check if the source directory exists
    if [ ! -d "$src" ]; then       
        return 1
    fi

    # Check for case sensitivity and existence of the destination directory
    if [ "$src" = "$dest" ]; then
       #echo "Source and destination are the same in a case-insensitive filesystem."
        return 1
    elif [ -d "$dest" ]; then
        #echo "Destination directory already exists: $dest"
        return 1
    fi

    # Perform the rename
    mv "$src" "$dest"
    if [ $? -eq 0 ]; then
        echo "Directory renamed successfully from $src to $dest"
    else
        echo "Failed to rename directory from $src to $dest"
        return 1
    fi
}

wait_for_postgres() {
    local host="$1"
    local port="$2"
    local max_attempts="$3"
    local wait_interval="$4"
    
    local attempt=0
    
    while [ $attempt -lt $max_attempts ]; do
        if pg_isready -h "$host" -p "$port" >/dev/null 2>&1; then
            echo "PostgreSQL is ready on $host:$port"
            return 0
        else
            attempt=$((attempt + 1))
            echo "Attempt $attempt: PostgreSQL is not yet ready on $host:$port. Retrying in $wait_interval seconds..."
            sleep "$wait_interval"
        fi
    done
    
    echo "Error: PostgreSQL on $host:$port is not ready after $max_attempts attempts."
    return 1
}

# Check if PUID or PGID is set to a non-root value
if [ "$PUID" -ne 0 ]; then
    if getent passwd $PUID > /dev/null 2>&1; then
        user_name=$(getent passwd $PUID | cut -d: -f1)
    else        
        useradd --uid $PUID -K UID_MIN=10 --comment "nonRootUser" --shell /bin/bash nonRootUser
    fi
fi

if [ "$PGID" -ne 0 ]; then
    if getent group $PGID > /dev/null 2>&1; then
        group_name=$(getent group $PGID | cut -d: -f1)
    else
        addgroup --gid $PGID --force-badname "nonRootGroup"
    fi
fi

rm -rf /config/hls
mkdir -p /config/Cache
mkdir -p /config/Logs
mkdir -p /config/PlayLists/EPG
mkdir -p /config/PlayLists/M3U
mkdir -p /config/HLS
mkdir -p $PGDATA

rename_directory /config/settings /config/Settings
rename_directory /config/backups /config/Backups

# Change ownership of the /app directory
if [ "$PUID" -ne 0 ] || [ "$PGID" -ne 0 ]; then
    echo "Changing ownership of /app to ${PUID:-0}:${PGID:-0}"
    chown -R ${PUID:-0}:${PGID:-0} /app
    echo "Changing ownership of /config to ${PUID:-0}:${PGID:-0}"
    find /config -mindepth 1 -maxdepth 1 -type d -not -path '/config/tv-logos' -not -path '/config/DB' -exec chown -R ${PUID:-0}:${PGID:-0} {} \;
fi

chown ${PUID:-0}:${PGID:-0} '/config/tv-logos' 2> /dev/null

# Pretty printing the configuration
echo "Configuration:"
echo "--------------"
echo "PGADMIN:"
echo "  Platform Type: $PGADMIN_PLATFORM_TYPE"
echo "  Setup Email: $PGADMIN_SETUP_EMAIL"
echo "  Setup Password: ********" #$PGADMIN_SETUP_PASSWORD"
echo "POSTGRES:"
echo "  User: $POSTGRES_USER"
echo "  Password: ********" #$POSTGRES_PASSWORD"
echo "  DB Name: $POSTGRES_DB"
echo "  Host: $POSTGRES_HOST"
echo "  Is DB Local: $POSTGRES_ISLOCAL"
echo "  Data Directory: $PGDATA"
echo "  Set Perms: $POSTGRES_SET_PERMS"
echo "OS:"
echo "  User: ${PUID:-0} Group: ${PGID:-0}"
echo "  User: postgres Group: postgres"
echo "  UID: $(id -u postgres) GID: $(id -g postgres)"

if [ "$POSTGRES_ISLOCAL" -eq 1 ] && [ "$POSTGRES_SET_PERMS" -eq 1 ]; then
    chown -R postgres:postgres "$PGDATA"
fi

# Check if any file is ready for restore and run restore.sh if so
if check_files_ready_for_restore; then
    # Print a warning message about the restoration process
    echo "WARNING: You are about to restore the database. This operation cannot be undone."
    echo "The restoration process will begin in 10 seconds. Press Ctrl+C to cancel."

    # Pause for 10 seconds to give the user a chance to cancel
    sleep 10
    
    echo "Initiating restoration process..."
    /usr/local/bin/restore.sh
else
    echo "No files ready for restoration."
fi

if [ "$POSTGRES_ISLOCAL" -eq 1 ]; then
    # Start the database
    /usr/local/bin/docker-entrypoint.sh postgres &

fi

 wait_for_postgres $POSTGRES_HOST "5432" 20 5
if [ $? -eq 0 ]; then
    # PostgreSQL is ready, you can proceed with your tasks
    echo "Postgres is up"
else
    # PostgreSQL is not ready after max_attempts, handle the error
    echo "Error: PostgreSQL is not ready."
    exit 1
fi

mkdir -p $BACKUP_DIR
mkdir -p $RESTORE_DIR

moveFilesAndDeleteDir /config/DB/Backup $BACKUP_DIR

chown ${PUID:-0}:${PGID:-0} $BACKUP_DIR
chown ${PUID:-0}:${PGID:-0} $RESTORE_DIR
chmod 777 $BACKUP_DIR
chmod 777 $RESTORE_DIR

if  [ "$PUID" -ne 0 ]; then
    if usermod -aG postgres "$user_name"; then
        echo "User $user_name added to group postgres successfully."
        chmod -R 775 $PGDATA
    else
        echo "Failed to add user $user_name to group postgres."
    fi
fi

# Execute the main application as the specified user
if [ "$PUID" -ne 0 ] && [ "$PGID" -ne 0 ]; then
    echo "Running as $user_name:$group_name"
    exec gosu $user_name:$group_name "$@"
elif [ "$PUID" -ne 0 ]; then
    echo "Running as $user_name"
    exec gosu $user_name "$@"
elif [ "$PGID" -ne 0 ]; then
    echo "Running as :$group_name"
    exec gosu :$group_name "$@"
else
    echo "Running as root"
    exec "$@"
fi